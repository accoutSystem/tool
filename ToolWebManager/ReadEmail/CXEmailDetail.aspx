﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CXEmailDetail.aspx.cs" Inherits="ToolWebManager.Email.CXEmailDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
     
    
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="刷新" Width="61px" />

        <div id="emailContent" runat="server"></div>
       
    </form>
</body>
</html>