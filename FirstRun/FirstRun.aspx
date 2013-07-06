<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="FirstRun.aspx.cs" Inherits="_min.FirstRun.FirstRun" %>
<%@ MasterType  virtualPath="~/Site.master"%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="MainPanel" runat="server">
            <asp:Table runat="server">

                <asp:TableRow>
                    <asp:TableCell>
                    <asp:Label runat="server" Text="Server Type" />
                    </asp:TableCell><asp:TableCell>
                        <asp:DropDownList runat="server" ID="ServerTypeDrop">
                            <asp:ListItem Value="" Text="--choose--" />
                            <asp:ListItem Value="MsSql" Text="MSSQL" />
                            <asp:ListItem Value="MySql" Text="MySQL" />
                        </asp:DropDownList>
                    </asp:TableCell></asp:TableRow><asp:TableRow>
                    <asp:TableCell>
                    <asp:Label runat="server" Text="Connection string" />
                    </asp:TableCell><asp:TableCell>
                        <asp:TextBox runat="server" ID="SystemConnstringTextBox" Width="500"></asp:TextBox>
                    </asp:TableCell></asp:TableRow><asp:TableRow>
                    <asp:TableCell>
                    <asp:Label runat="server" Text="Username" />
                    </asp:TableCell><asp:TableCell>
                        <asp:TextBox runat="server" ID="UsernameTextBox" Width="200"></asp:TextBox>
                    </asp:TableCell></asp:TableRow><asp:TableRow>
                    <asp:TableCell>
                    <asp:Label runat="server" Text="Password" />
                    </asp:TableCell><asp:TableCell>
                        <asp:TextBox runat="server" ID="PasswordTextBox" Width="200"></asp:TextBox>
                    </asp:TableCell></asp:TableRow><asp:TableRow>
                    <asp:TableCell>
                    <asp:Label runat="server" Text="Retype password" />
                    </asp:TableCell><asp:TableCell>
                        <asp:TextBox runat="server" ID="RetypePasswordTextBox" Width="200"></asp:TextBox>
                    </asp:TableCell></asp:TableRow><asp:TableRow>
                    <asp:TableCell>
                        <asp:Button runat="server" ID="SaveButton" Text="Configure Webmin" OnClick="SaveButton_Click" /></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:BulletedList runat="server" ID="Errors" />
                    </asp:TableCell></asp:TableRow></asp:Table></asp:Panel></asp:Content>