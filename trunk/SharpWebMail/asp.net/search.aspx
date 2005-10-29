<%@ Page Language="C#"  Inherits="anmar.SharpWebMail.UI.Search" trace="false"%>
<%@ Register TagPrefix="SharpUI" TagName="globalUI" Src="globalUI.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<title><%=Application["product"]%></title>
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
<link rel="stylesheet" type="text/css" href="sharpwebmail.css">
</head>
<body bgcolor="#FFFFFF" text="#000000" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0">
<form id="sharpwebmailform" method="post" runat="server">
<SharpUI:globalUI id="SharpUI" runat="server">
<CentralPanel>
					<table width="90%" cellpadding="0" cellspacing="0" align="center" height="95%">
                      <tr>
                        <td class="XPPanelTitleBorderLeft"><img src="images/spacer.gif" width="6" height="3"></td>
                        <td width="100%" class="XPPanelTitleBorder" valign="middle"></td>
                        <td class="XPPanelTitleBorderRight"><img src="images/spacer.gif" width="6" height="3"></td>
                      </tr>
                      <tr>
                        <td class="XPPanelTitleBorderLeft"><img src="images/spacer.gif" width="6" height="22"></td>
                        <td width="100%" class="XPPanel" valign="middle"> 
                          <table width="100%" border="0" cellspacing="0" cellpadding="0" class="XPPanelTitle" align="left">
                            <tr>
                              <td width="100%" class="XPPanel"><asp:Label id="searchWindowTitle" runat="server"/></td>
                              <td align="right" nowrap><img src="images/PanelClose.gif" width="11" height="11" align="middle"><img src="images/spacer.gif" width="3" height="14"></td>
                          </tr>
                        </table>
                        </td>
                        <td class="XPPanelTitleBorderRight"><img src="images/spacer.gif" width="6" height="4"></td>
                      </tr>
                      <tr>
                        <td class="XPPanelBorderTopLeft"><img src="images/spacer.gif" width="6" height="5"></td>
                        <td width="100%" class="XPPanelBorderTop" valign="middle"></td>
                        <td class="XPPanelBorderTopRight"><img src="images/spacer.gif" width="6" height="5"></td>
                      </tr>
                      <tr> 
                        <td class="XPPanelBorder"><img src="images/spacer.gif" width="6" height="4"></td>
                        <td width="100%" class="XPWindowBackground"> 
                          <table align="center" width="97%" cellpadding="0" cellspacing="0">
                            <tr> 
                              <td align="left" width="1%" nowrap> <asp:Label id="newMessageWindowFromNameLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td nowrap><img src="images/spacer.gif" width="4" height="1"> <input id="fromsearch" type="text" runat="server" value="" style="XPInput" tabindex="10" name="text" /></td>
                              <td width="100%"><img src="images/spacer.gif" width="15" height="1"></td>
                            </tr>
                            <tr> 
                              <td align="left" width="1%" nowrap> <asp:Label id="newMessageWindowSubjectLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td nowrap><img src="images/spacer.gif" width="4" height="1"> <input id="subjectsearch" type="text" runat="server" value="" style="XPInput" tabindex="10" name="text" /></td>
                              <td width="100%"><img src="images/spacer.gif" width="15" height="1"></td>
                            </tr>
                            <tr> 
                              <td><img src="images/spacer.gif" width="1" height="30"></td>
                              <td align="right" valign="bottom"><asp:button id="searchWindowSearchButton" runat="server" text="Search" OnClick="Search_Click" tabindex="10" /></td>
                              <td></td>
                            </tr>
                          </table>
                        </td>
                        <td class="XPPanelBorder"><img src="images/spacer.gif" width="6" height="6"></td>
                      </tr>
                      <tr> 
                        <td class="XPPanelFooterLeft"><img src="images/spacer.gif" width="6" height="8"></td>
                        <td class="XPPanelFooter"></td>
                        <td class="XPPanelFooterRight"><img src="images/spacer.gif" width="6" height="6"></td>
                      </tr>
                    </table>
	</CentralPanel>
</SharpUI:globalUI>
</form>
</body>
</html>
