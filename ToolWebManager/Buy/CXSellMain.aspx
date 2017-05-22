<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CXSellMain.aspx.cs" Inherits="ToolWebManager.Buy.CXSellMain" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style>
        body {
            font-size: 12px;
        }
    </style>
    <script type="text/javascript">


    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            欢迎您:<asp:Label ID="lbUser" runat="server" Text="未知用户" Font-Bold="True" Font-Size="14pt" ForeColor="#CC3300"></asp:Label>
            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click">安全退出</asp:LinkButton>
            <br />
            <br />
            您剩余金额:<asp:Label ID="lbSellMonry" runat="server" Text="0" Font-Bold="True" Font-Size="14pt" ForeColor="#CC3300"></asp:Label>
            <br />
            <br />
            普通账号单价:<asp:Label ID="lbPlainPrice" runat="server" Font-Bold="True" Font-Size="14pt" ForeColor="#CC3300" Text="0"></asp:Label>
            <br />
            <br />
            手机核验账号单价:<asp:Label ID="lbMobilePrice" runat="server" Font-Bold="True" Font-Size="14pt" ForeColor="#CC3300" Text="0"></asp:Label>
            <br />
            <br />
            可导出普通账号:<asp:Label ID="lbPTUser" runat="server" Text="0" Font-Bold="True" Font-Size="14pt" ForeColor="#CC3300"></asp:Label>
            <br />
            <br />
            可导出手机核验账号:<asp:Label ID="lbMobileUser" runat="server" Text="0" Font-Bold="True" Font-Size="14pt" ForeColor="#CC3300"></asp:Label>
            <br />
            <br />
            请输入导出数量<asp:DropDownList ID="DropDownList1" runat="server">
            </asp:DropDownList>
            :<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
            <asp:CheckBox ID="CheckBox1" runat="server" Text="导出手机资源" Checked="True" />
            <asp:CheckBox ID="CheckBox2" runat="server" Text="正序" />
            &nbsp;<asp:Button ID="btnexport" runat="server" Text="导出" OnClientClick="return confirm('您确定导出指定数量的资源吗？');" OnClick="btnexport_Click" />
            &nbsp;
        <a href="CXSellDetail.aspx" target="_blank">导出明细,提供重新下载</a>
            <br />
            <br />
            <asp:Label ID="Label1" runat="server" Text="1:导出的资源如果需要添加联系人请联系管理员为资源添加联系人。&lt;br /&gt;&lt;br /&gt;2:为防止资源提取换乱在其中一个代理商导出的时候其他代理商将排队等待。&lt;br /&gt;&lt;br /&gt;3:导出格式为csv,可以直接使用，如需美观请代理商另存为xsl格式后调整格式" Font-Bold="True" ForeColor="#CC3300"></asp:Label>

            <br />
            <br />
            <asp:Label ID="Label2" runat="server" Text="调试信息:"></asp:Label>
            <asp:Label ID="lbDebug" runat="server" Text="..."></asp:Label>
            <br />

        </div>
    </form>
</body>
</html>
