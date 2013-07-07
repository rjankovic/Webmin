<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="FirstRun.aspx.cs" Inherits="_min.FirstRun.FirstRun" %>

<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="MainPanel" runat="server">
        <table>

            <tr>
                <td>
                    <asp:Label runat="server" Text="Server Type" />
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ServerTypeDrop">
                        <asp:ListItem Value="" Text="--choose--" />
                        <asp:ListItem Value="MsSql" Text="MSSQL" />
                        <asp:ListItem Value="MySql" Text="MySQL" />
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" Text="Connection string" />
                </td>
                <td>
                    <asp:TextBox runat="server" ID="SystemConnstringTextBox" Width="500"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" Text="Username" />
                </td>
                <td>
                    <asp:TextBox runat="server" ID="UsernameTextBox" Width="200"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" Text="Password" />
                </td>
                <td>
                    <asp:TextBox runat="server" ID="PasswordTextBox" Width="200"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" Text="Retype password" />
                </td>
                <td>
                    <asp:TextBox runat="server" ID="RetypePasswordTextBox" Width="200"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" Text="Email" />
                </td>
                <td>
                    <asp:TextBox runat="server" ID="MailTextBox" Width="200"></asp:TextBox>
                </td>
            </tr>
            <td>
                <asp:Button runat="server" ID="SaveButton" Text="Configure Webmin" OnClick="SaveButton_Click" /></td>
            </tr>
                <tr>
                    <td colspan="2">
                        <asp:BulletedList runat="server" ID="Errors" />
                    </td>
                </tr>
        </table>
    </asp:Panel>
</asp:Content>
