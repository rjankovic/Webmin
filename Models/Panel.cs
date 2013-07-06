using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _min.Interfaces;
using _min.Common;
using System.Data;
using System.Runtime.Serialization;
using System.IO;

namespace _min.Models
{
    [DataContract]      // dont serialize iiner object - controls and fields
    public class Panel
    {
        [DataMember]
        public string tableName { get; private set; }
        [IgnoreDataMember]
        public List<Panel> children { get; private set; }
        [IgnoreDataMember]
        public List<IField> fields { get; private set; }    // including Docks
        [IgnoreDataMember]
        public List<Control> controls { get; private set; }
        [DataMember]
        public List<string> PKColNames { get; set; }
        [IgnoreDataMember]
        public DataRow PK { get; set; }
        [IgnoreDataMember]
        public DataRow OriginalData { get; set; }
        // the retrieved datarow without the columns that arent managed by a field in the panel
        [IgnoreDataMember]
        public DataRow RetrievedManagedData { get; private set; }
        [IgnoreDataMember]
        public DataRow RetrievedInsertData { get; private set; }
        [IgnoreDataMember]
        private Panel _parent;
        [IgnoreDataMember]
        public Panel parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (_parent == null)
                    _parent = value;
                else throw new Exception("Panel already set");
            }
        }

        [IgnoreDataMember]
        public int panelId { get; set; }
        [DataMember]
        public PanelTypes type { get; set; }
        [DataMember]
        public string panelName { get; set; }
        [DataMember]
        public bool isBaseNavPanel { get; set; }
        [DataMember]
        public int? idHolder { get; set; }
        [DataMember]
        public int displayAccessRights { get; set; }

        public Panel(string tableName, int panelId, PanelTypes type, List<Panel> children,
            List<IField> fields, List<Control> controls, List<string> PKColNames, DataRow PK = null, Panel parent = null)
        {
            this.tableName = tableName;
            this.panelId = panelId;
            this.children = children;
            this.fields = fields;
            this.controls = controls;
            this.PKColNames = PKColNames;
            this.PK = PK;
            this.parent = parent;
            this.type = type;
            if (this.controls == null) this.controls = new List<Control>();
            if (this.fields == null) this.fields = new List<IField>();
            if (this.PKColNames == null) this.PKColNames = new List<string>();
            if (this.children == null) this.children = new List<Panel>();
            foreach (Panel child in this.children)
            {
                child.parent = this;
            }
            foreach (IField f in this.fields)
            {
                f.Panel = this;
            }
            foreach (Control c in this.controls)
            {
                c.panel = this;
            }
        }

        public string Serialize()
        {
            MemoryStream ms = new MemoryStream();
            DataContractSerializer ser = new DataContractSerializer(typeof(Panel));
            ser.WriteObject(ms, this);

            return Functions.StreamToString(ms);
        }

        public void AddChildren(List<Panel> children)
        {
            if (children.Count > 0 && this.children == null) this.children = new List<Panel>();
            foreach (Panel p in children)
            {
                if (this.children.Any(p2 => p2.panelId == p.panelId))
                    throw new Exception("Panel already contains a child with this id.");
                this.children.Add(p);
                p.parent = this;
            }
        }

        public void AddChild(Panel p)
        {
            if (this.children.Any(p2 => p2.panelId == p.panelId))
                throw new Exception("Panel already contains a child with this id.");
            this.children.Add(p);
            p.parent = this;
        }

        /// <summary>
        /// Set the initial id for the panel after it has been saved to the database 
        /// and a new AutoIncrement - the panel`s ID - has been generated.
        /// </summary>
        /// <param name="id"></param>
        public void SetCreationId(int id)
        {
            if (panelId == 0) panelId = id;
            else
                throw new Exception("Panel id already initialized");
            fields.ForEach(x => x.RefreshPanelId());
            controls.ForEach(x => x.RefreshPanelId());
        }

        public void SetParentPanel(Panel parentPanel)
        {
            if (parent == null) parent = parentPanel;
            else
                throw new Exception("Panel parent already initialized");
        }

        /// <summary>
        /// Create a both-side binding between this panel and the fields given
        /// </summary>
        /// <param name="fields"></param>
        public void AddFields(List<IField> fields)
        {
            foreach (IField newField in fields)
            {
                if (this.fields.Any(f => f.FieldId == newField.FieldId && this.panelId != 0 && f.FieldId != null && f.FieldId != 0))
                    throw new Exception("Panel already contains a field with this id.");
                this.fields.Add(newField);
                newField.Panel = this;
            }
        }

        /// <summary>
        /// Add controls to the panel`s Controls collection and the control`s panel property to this panel (both-side binding).
        /// </summary>
        /// <param name="controls"></param>
        public void AddControls(List<Control> controls)
        {
            foreach (Control newControl in controls)
            {
                int dupl = 0;
                if (this.panelId != 0)
                    dupl = (from c in controls where c.action == newControl.action && c != newControl select c).Count();
                if (Convert.ToInt32(dupl) > 0)
                    throw new Exception("Panel already contains a control for this action.");
                this.controls.Add(newControl);
                newControl.panel = this;
            }
        }

        /// <summary>
        /// sets panel`s controls, fields and children to empty lists
        /// </summary>
        public void InitAfterDeserialization()
        {
            if (this.children != null || this.fields != null || this.controls != null)
                throw new Exception("Some of the collections have already been initializaed");
            this.children = new List<Panel>();
            this.fields = new List<IField>();
            this.controls = new List<Control>();
        }


        /// <summary>
        /// collects data from its fields and fillst them into a DataRow so that it is prepared for being passed to the database driver for UPDATE / INSERT commands
        /// </summary>
        public void RetrieveDataFromFields()
        {
            // form two tables to match the structure of data included in the fields
           
            // one suitable for update commands
            DataTable tbl = new DataTable();

            // one for inserts (does not contain AI ...)
            DataTable insTbl = new DataTable();
            if (PK != null)
            {
                foreach (DataColumn col in PK.Table.Columns)
                    tbl.Columns.Add(new DataColumn(col.ColumnName, col.DataType));
            }
            foreach (IField f in fields)
            {
                if (!(f is IColumnField)) continue;
                IColumnField cf = f as IColumnField;
                if (cf.Data != null && cf.Data != DBNull.Value)
                {
                    // do not create  duplicities - if the column is a part of the primary key, do not add it to the update table again. 
                    // Moreover, the PK of a record should not change
                    if (PK == null || !PK.Table.Columns.Contains(cf.ColumnName))
                        tbl.Columns.Add(new DataColumn(cf.ColumnName, cf.Data.GetType()));
                    insTbl.Columns.Add(new DataColumn(cf.ColumnName, cf.Data.GetType()));
                }
                else
                {
                    // columns without value vill be passed as ints - it will not matter in the end as the command will be concatenated from string parts
                    if (PK == null || !PK.Table.Columns.Contains(cf.ColumnName))
                        tbl.Columns.Add(new DataColumn(cf.ColumnName, typeof(int)));
                    insTbl.Columns.Add(new DataColumn(cf.ColumnName, typeof(int)));
                }
            }

            // create DataRows from these new tables
            RetrievedManagedData = tbl.NewRow();
            RetrievedInsertData = insTbl.NewRow();
            if (PK != null)
            {
                // add the PK finally
                foreach (DataColumn col in PK.Table.Columns)
                    RetrievedManagedData[col.ColumnName] = PK[col.ColumnName];
            }

            // fill the DataRows with data
            foreach (IField f in fields)
            {
                if (!(f is IColumnField)) continue;
                IColumnField cf = f as IColumnField;
                if (cf.Data != null && (cf.Data.ToString() != ""))      // dangerous?
                {
                    RetrievedManagedData[cf.ColumnName] = cf.Data;
                    RetrievedInsertData[cf.ColumnName] = cf.Data;
                }
                else
                {
                    RetrievedManagedData[cf.ColumnName] = DBNull.Value;
                    RetrievedInsertData[cf.ColumnName] = DBNull.Value;
                }
            }
        }
    }
}
