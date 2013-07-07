<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="ProjectDetail.aspx.cs" Inherits="_min.Sys.ProjectDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <asp:DetailsView ID="DetailsView" runat="server" AutoGenerateRows="False" 
        DefaultMode="Insert"
        onitemcommand="DetailsView_ItemCommand" 
        BorderStyle="None" CssClass="projectDetail" GridLines="None" 
        oniteminserting="DetailsView_ItemInserting" 
        onitemupdating="DetailsView_ItemUpdating" OnDataBound="DetailsView_DataBound">
        <Fields>
            <asp:BoundField DataField="Name" HeaderText="Name" />
            <asp:BoundField DataField="ConnstringWeb" 
                HeaderText="Connection to the website" />
            <asp:ButtonField ButtonType="Button" CommandName="TestWeb" 
                Text="Test the connection" />
            <asp:BoundField DataField="ConnstringIS" 
                HeaderText="Connection to the information shcema" />
            <asp:ButtonField ButtonType="Button" CommandName="TestIS" 
                Text="Test the connection" />
                <asp:CommandField ButtonType="Button" ShowCancelButton="true" 
            ShowEditButton="true" ShowInsertButton="true" />
            <asp:TemplateField HeaderText="Database server type">
                <EditItemTemplate> 
                <asp:DropDownList ID="ddlServerType" runat="server" 
                    AppendDataBoundItems="True" > 
                        <asp:ListItem Value="" Text="" /> 
                    </asp:DropDownList> 
                </EditItemTemplate> 
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>
    <asp:Button ID="DeleteButton" runat="server" Text="Delete" Visible="false" 
    OnClientClick="return confirm('Do you really want to delete the project, the whole saved architecture and revoke all the access rights delegated to users?')"
     CommandName="Delete" onclick="DeleteButton_Click"
    />


            <asp:BulletedList ID="InfoList" runat="server">
            </asp:BulletedList>

</asp:Content>
