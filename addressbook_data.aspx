<%@ Page Language="C#"  Inherits="anmar.SharpWebMail.UI.Pages.AddressBookData" trace="false" validateRequest="true" %>
<%@ Register TagPrefix="SharpUI" TagName="globalUI" Src="globalUI.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
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
                    <table width="95%" cellpadding="0" cellspacing="0" align="center" height="95%">
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
                              <td width="80%" align="left" class="XPPanel"><asp:Label id="addressbookImportExport" runat="server"/></td>
							  <td width="20%" align="right" class="XPPanel">
                                <table width="100%"  border="0">
                                  <tr>
								    <td width="100%"><img src="images/spacer.gif" width="1" height="1"></td>
                                    <td></td>
                                  </tr>
                                </table></td>
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
						<asp:PlaceHolder id="newMessagePH" runat="server"> 
                          <table align="center" width="97%">
                            <tr>
                              <td align="right" nowrap width="1%"><asp:Label id="addressbookLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> 
                                <asp:Label id="addressbookLabelItem" CssClass="XPFormText" runat="server"/>
                              </td>
                              <td> </td>
                            </tr>
                            <tr>
                              <td align="right" nowrap width="1%"><asp:Label id="addressbookDelimiterLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> 
                                <input id="delimiter" type="text" runat="server"  maxlength="2" value="," class="XPInput" tabindex="10" />
								<ASP:RequiredFieldValidator id="ReqNameValidator" runat="server" ErrorMessage="*" Display="Static" ControlToValidate="delimiter" /> 
                              </td>
                              <td> </td>
                            </tr>
                            <tr>
                              <td align="right" nowrap width="1%"><asp:Label id="addressbookImportDuplicatesLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> 
                                <input type="checkbox" ID="duplicates" runat="server" tabindex="10" />
                              </td>
                              <td> </td>
                            </tr>
                            <tr>
                              <td valign="top" align="right" nowrap><asp:Label id="addressbookImportExportData" CssClass="XPFormLabel" runat="server"/></td>
                              <td> 
                                <textarea id="data" cols="50" rows="14" type="text" wrap="off" runat="server" tabindex="10" />
                              </td>
                              <td> </td>
                            </tr>
                            <tr>
                              <td>&nbsp;</td>
                              <td align="left">
                              	<asp:button id="addressbookImport" CausesValidation="true" OnClick="AddressImport_Click" runat="server" tabindex="10" />
                              	<img src="images/spacer.gif" width="6" height="1">
                              	<asp:button id="addressbookExport" CausesValidation="true" OnClick="AddressExport_Click" runat="server" tabindex="10" />
                              </td>
                              <td></td>
                            </tr>
                          </table>
                          </asp:PlaceHolder>
						  <asp:PlaceHolder id="ConfirmationPH" Visible="false" runat="server"> 
                          <table align="center">
                            <tr> 
                              <td></td>
                              <td align="center"><asp:Label id="ConfirmationMessage" CssClass="XPFormLabel" runat="server"/> 
                              </td>
                              <td> </td>
                            </tr>
                          </table>
                          </asp:PlaceHolder>
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
