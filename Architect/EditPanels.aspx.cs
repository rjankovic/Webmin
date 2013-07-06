﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using _min.Common;
using _min.Interfaces;
using _min.Models;
using System.Configuration;
using System.Text.RegularExpressions;

using _min.Navigation;
using CC = _min.Common.Constants;
using CE = _min.Common.Environment;
using MPanel = _min.Models.Panel;
using _min.Controls;

namespace _min.Architect
{
    /// <summary>
    /// Edit panels on global level - remove existing panels for tables or add panels for table that isn`t edited.
    /// Only panels inaccessible from menu can be removed and only tables that have at least one part of PK as a independent (non-foreign key) column, 
    /// can have panels. Panels are always added / removed in pairs summary panel / edit panel.
    /// </summary>
    public partial class EditPanels : System.Web.UI.Page
    {
        DataTable summary;
        MinMaster mm;

        protected void Page_Init(object sender, EventArgs e)
        {
            mm = (MinMaster)Master;
            
            if (!(Session["Summary"] is DataTable))
            {
                HierarchyNavTable baseNavTable = ((TreeControl)(mm.SysDriver.MainPanel.controls[0])).storedHierarchyData;

                List<string> tables = mm.Stats.Tables;

                // the summary of acessibility/dependency/... is generated at the begining, afterwards it is restroed from the Session
                summary = new DataTable();
                summary.Columns.Add("TableName", typeof(string));
                summary.Columns.Add("Independent", typeof(bool));
                summary.Columns.Add("HasPanels", typeof(bool));
                summary.Columns.Add("Reachable", typeof(bool));
                foreach (string tableName in mm.Stats.Tables)
                {
                    if(!mm.Stats.PKs.ContainsKey(tableName))
                        continue;
                    DataRow r = summary.NewRow();
                    r["TableName"] = tableName;     // get it only once - table is stored in Session and updated

                    // this will take a bit longer as the primary key needs to be determined based on the information schema
                    // A table for which the primary key is at least partly also a foreign key is dependant.
                    r["Independent"] = !(mm.Stats.PKs[tableName].Any(pkCol => mm.Stats.FKs[tableName].Any(fk => fk.myColumn == pkCol)));

                    List<MPanel> tablePanels = (from MPanel p in mm.SysDriver.Panels.Values where p.tableName == tableName select p).ToList<MPanel>();
                    r["HasPanels"] = tablePanels.Count > 0;     // now surely equal to 2 (panels are added/removed in pairs)
                    r["Reachable"] = false;
                    if ((bool)(r["HasPanels"]))
                    {
                        r["Reachable"] = baseNavTable.Select("NavId IN (" + tablePanels[0].panelId + ", " + tablePanels[1].panelId + ")").Length > 0;
                    }
                    summary.Rows.Add(r);
                }

                Session["Summary"] = summary;
            }
            else {
                summary = (DataTable)Session["Summary"];
            }



            if (!Page.IsPostBack)
            {
                // the next time grid is set like this will be after panels addition / removal
                TablesGrid.DataSource = summary;
                TablesGrid.DataBind();
                ResetActionClickablility();
            }

            BackButton.PostBackUrl = BackButton.GetRouteUrl("ArchitectShowRoute", new { projectName = _min.Common.Environment.project.Name });

        }

        /// <summary>
        /// Called after a modification of panels that results in a different summary, this method changes the options for each table as follows:
        /// 1. disable "Add" for tables that either have panels (have already been added) or are dependent
        /// 2. disabler "Remove" for tables that either have reachable panels or have no panels at all - have been removed
        /// </summary>
        private void ResetActionClickablility() {
            for (int i = 0; i < summary.Rows.Count; i++)
            {
                // this is not really needed, as the control is recreated, but kept for clarity and to be sure
                ((LinkButton)(TablesGrid.Rows[i].Cells[0].Controls[0])).Enabled = true;
                ((LinkButton)(TablesGrid.Rows[i].Cells[1].Controls[0])).Enabled = true;

                //1
                if ((bool)summary.Rows[i]["HasPanels"] || !(bool)summary.Rows[i]["Independent"])
                    ((LinkButton)(TablesGrid.Rows[i].Cells[0].Controls[0])).Enabled = false;
                //2
                if ((bool)summary.Rows[i]["Reachable"] || !(bool)summary.Rows[i]["HasPanels"])
                    ((LinkButton)(TablesGrid.Rows[i].Cells[1].Controls[0])).Enabled = false;
                
            }
        }

        
        /// <summary>
        /// When one of the action buttons beside a table is clicked, the "Confirm" button is enabled,
        /// the color of the containing row is changed to green (add) / red (remove) and in the case of "Add",
        /// M2N mappings that map this table to some other are found via the StatsDriver and displayed below as ChceckBoxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TablesGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in TablesGrid.Rows)
                row.BackColor = System.Drawing.Color.White;
            SaveButton.Enabled = true;
            int index = TablesGrid.SelectedIndex;
            if ((bool)summary.Rows[index]["HasPanels"])
            {   // remove
                TablesGrid.SelectedRow.BackColor = System.Drawing.Color.OrangeRed;
                MappingsLabel.Visible = false;
                MappingsCheck.Visible = false;
            }
            else {      // mapping optiona for new panel
                TablesGrid.SelectedRow.BackColor = System.Drawing.Color.LightGreen;
                
                // check in the sense of checkbock list - no validation
                MappingsCheck.DataSource = from M2NMapping mp in mm.Stats.Mappings[summary.Rows[index]["TableName"] as string] select 
                                               new { text = "Mapping to " + mp.refTable + " via " + mp.mapTable, mapTable = mp.mapTable };
                MappingsCheck.DataTextField = "text";
                MappingsCheck.DataValueField = "mapTable";
                MappingsCheck.DataBind();
                if (MappingsCheck.Items.Count > 0)
                {
                    MappingsLabel.Visible = true;
                    MappingsCheck.Visible = true;
                }
            }
        }


        protected void SaveButton_Click(object sender, EventArgs e)
        {
            mm.SysDriver.FullProjectLoad();
            int index = TablesGrid.SelectedIndex;
            string tableName = summary.Rows[index]["TableName"] as string;
            
            if ((bool)summary.Rows[index]["HasPanels"])     // then remove is the only option that could have been voted for
            {
                IEnumerable<MPanel> toRemove = from MPanel p in mm.SysDriver.Panels.Values where p.tableName == tableName select p;
                foreach (MPanel p in toRemove)
                    mm.SysDriver.RemovePanel(p);
                summary.Rows[index]["HasPanels"] = false;
            }
            else
            {   // otherwise add new editation a navigation panel for this table
                mm.Architect.mappings = mm.Stats.Mappings[tableName];
                mm.Architect.hierarchies = new List<string>();  // to speed it up, hierarchy nvigation can be set in panel customization
                MPanel editPanel = mm.Architect.ProposeForTable(tableName);
                MPanel summaryPanel = mm.Architect.proposeSummaryPanel(tableName);


                summaryPanel.SetParentPanel(mm.SysDriver.MainPanel);       // add to db
                editPanel.SetParentPanel(mm.SysDriver.MainPanel);

                foreach (_min.Models.Control c in editPanel.controls) {
                    c.targetPanel = summaryPanel;
                }
                foreach (_min.Models.Control c in summaryPanel.controls) {
                    c.targetPanel = editPanel;
                }

                summaryPanel.panelName = "Summary of " + tableName;
                editPanel.panelName = "Editation of " + tableName;
                mm.SysDriver.BeginTransaction();
                mm.SysDriver.AddPanels(new List<_min.Models.Panel> { summaryPanel, editPanel });
                mm.SysDriver.CommitTransaction();
                foreach (_min.Models.Control c in summaryPanel.controls)    // simlified for now
                {
                    c.targetPanel = editPanel;
                }
                foreach (_min.Models.Control c in editPanel.controls)
                {
                    c.targetPanel = summaryPanel;
                }
                
                summary.Rows[index]["HasPanels"] = true;
            }
            // rebuild the grid
            TablesGrid.DataSource = summary;
            TablesGrid.DataBind();
            ResetActionClickablility();
            TablesGrid.SelectedRow.BackColor = System.Drawing.Color.White;
            TablesGrid.SelectedRowStyle.BackColor = System.Drawing.Color.White;
            TablesGrid.SelectedIndex = -1;
            SaveButton.Enabled = false;
            mm.SysDriver.IncreaseVersionNumber();

            Session["summary"] = summary;
        }

    }

}