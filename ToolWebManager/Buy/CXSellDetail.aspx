<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CXSellDetail.aspx.cs" Inherits="ToolWebManager.Buy.CXSellDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>  <style  >
        body {
             font-size:12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="刷新" />
        <br />
    
        <asp:GridView ID="GridView1" runat="server" BackColor="#CCCCCC" BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" ForeColor="Black" AutoGenerateColumns="False" CellSpacing="2" Width="893px">
            <Columns>
                <asp:BoundField DataField="createtime" HeaderText="创建时间" >
                <ItemStyle Width="120px" />
                </asp:BoundField>
                <asp:BoundField DataField="sellNumber" HeaderText="导出数量" >
        
                <ItemStyle Width="60px" />
                </asp:BoundField>
        
                <asp:BoundField DataField="sellMoney" HeaderText="扣款(元)" >
                       <ItemStyle Width="60px" />
                </asp:BoundField>
                       <asp:BoundField DataField="sellType" HeaderText="账号类型" >
                 <ItemStyle Width="90px" />
                </asp:BoundField>
                 <asp:TemplateField HeaderText="资源名(点击下载)">
                    <ItemTemplate>
                       <a    target="_blank" href="<%#Eval("downLoadAddress") %>"><%#Eval("address") %></a>
                      
                    </ItemTemplate>

                </asp:TemplateField>
                  <asp:TemplateField HeaderText="资源名(点击下载txt)">
                    <ItemTemplate>
                       <a    target="_blank" href="<%#Eval("txtDownLoadAddress") %>">下载txt版本</a>
                    </ItemTemplate>

                </asp:TemplateField>
            </Columns>
            <FooterStyle BackColor="#CCCCCC" />
            <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Left" />
            <RowStyle BackColor="White" />
            <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F1F1F1" />
            <SortedAscendingHeaderStyle BackColor="#808080" />
            <SortedDescendingCellStyle BackColor="#CAC9C9" />
            <SortedDescendingHeaderStyle BackColor="#383838" />
        </asp:GridView>
    
    </div>
    </form>
</body>
</html>
