<%@ Page Language="C#" Inherits="anmar.SharpWebMail.UI.Login" trace="false"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
<title>Login - <%=Application["product"]%></title>
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
<link rel="stylesheet" type=text/css href="sharpwebmail.css">
</head>
<body bgcolor="#FFFFFF" text="#000000">
    <p>&nbsp;</p>
<form runat="server" ID="LoginForm" method="post">
  <table width="60%" cellpadding="0" cellspacing="0" align="center">
    <tr>
    <td colspan="3"> 
      <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
          <td class="XPTitleBarLeft"><img src="images/spacer.gif" width="6" height="30"></td>
          <td class="XPTitleBar"><img src="images/spacer.gif" width="10" height="1"></td>
          <td class="XPTitleBar" width="100%"><asp:Label EnableViewState="false" id="loginWindowTitle" runat="server" /></td>
          <td class="XPTitleBar"></td>
          <td class="XPTitleBarClose"><img src="images/spacer.gif" width="21" height="30"></td>
          <td class="XPTitleBarRight"><img src="images/spacer.gif" width="6" height="30"></td>
        </tr>
      </table>
    </td>
  </tr>
  <tr>
    <td class="XPBorderLeft"></td>
    <td width="100%" class="XPWindowBackground"> 
        <table align="center">
          <tr> 
            <td> <asp:Label EnableViewState="false" id="loginWindowUsername" CssClass="XPFormLabel" runat="server" /></td>
            <td> 
              <input id="username" type="text" runat="server" value="" size="25" style="XPInput" name="text" />
            </td>
            <td> <ASP:RequiredFieldValidator EnableViewState="false" id="RequiredFieldValidator1" runat="server" ErrorMessage="*" Display="Static" ControlToValidate="username" /><ASP:RegularExpressionValidator id="usernameValidator" ValidationExpression="^^[A-z0-9_\-]+[@][A-z0-9_\-]+([.][A-z0-9_\-]+){0,}[.][A-z]{2,4}$" ControlToValidate="username" runat="server" ErrorMessage="*" Display="Static" />
            </td>
          </tr>
          <tr> 
            <td> <asp:Label EnableViewState="false" id="loginWindowPassword" CssClass="XPFormLabel" runat="server" /></td>
            <td> 
              <input id="password" type="password" runat="server" style="XPInput" size="25" name="password" />
            </td>
            <td> <ASP:RequiredFieldValidator EnableViewState="false" id="RequiredFieldValidator2" runat="server" ErrorMessage="*" Display="Static" ControlToValidate="password" />
            </td>
          </tr>
          <tr>
            <td>&nbsp;</td>
            <td align="center"><asp:button EnableViewState="false" id="Button1" onclick="Login_Click" runat="server" text="Login" />
            </td>
            <td>&nbsp;</td>
          </tr>
        </table>
        <p align="center"><asp:Label EnableViewState="false" id="errorMsgLogin" runat="server" CssClass="XPErrorMessage" visible="false" /></p>
    </td>
    <td class="XPBorderRight"></td>
  </tr>
  <tr>
    <td class="XPBorderLeft"><img src="images/spacer.gif" width="4" height="4"></td>
    <td class="XPWindowFoot"><a href="http://anmar.eu.org/projects/sharpwebmail/" class="XPWindowFoot" target="_blank"><%=Application["product"]%></a> - Copyright &copy; 2003 - 2004 <a href="http://anmar.eu.org" class="XPWindowFoot" target="_blank">Angel Marin</a> All rights reserved</td>
    <td class="XPBorderRight"><img src="images/spacer.gif" width="4" height="4"></td>
  </tr>
  <tr>
    <td class="XPFooterLeft"><img src="images/spacer.gif" width="4" height="4"></td>
    <td class="XPFooter"></td>
    <td class="XPFooterRight"><img src="images/spacer.gif" width="4" height="4"></td>
  </tr>
</table>
</form>
</body>
</html>
