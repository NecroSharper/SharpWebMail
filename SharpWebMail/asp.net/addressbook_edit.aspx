<%@ Page Language="C#"  Inherits="anmar.SharpWebMail.UI.AddressBookEdit" trace="false" validateRequest="true" %>
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
                              <td width="80%" align="left" class="XPPanel"><asp:Label id="addressbookEntryTitle" runat="server"/></td>
							  <td width="20%" align="right" class="XPPanel">
                                <table width="100%"  border="0">
                                  <tr>
								    <td width="100%"><img src="images/spacer.gif" width="1" height="1"></td>
                                    <td>
                                      <asp:ImageButton runat="server" ID="msgtoolbarDelete" causesvalidation="false" OnCommand="AddressToolbarCommand" CommandName="delete"
                                           ImageUrl="images/msgtoolbar_delete.gif"/>
                                     </td>
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
                              <td align="right" nowrap width="1%"><asp:Label id="addressbookNameLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> 
                                <input id="name" type="text" runat="server" value="" class="XPInputEX" tabindex="10" />
								<ASP:RequiredFieldValidator id="ReqNameValidator" runat="server" ErrorMessage="*" Display="Static" ControlToValidate="name" /> 
                              </td>
                              <td> </td>
                            </tr>
                            <tr> 
                              <td align="right" nowrap><asp:Label id="addressbookEmailLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> 
                                <input id="email" type="text" runat="server" value="" class="XPInputEX" tabindex="10" />
								<ASP:RequiredFieldValidator id="ReqEmailValidator" runat="server" ErrorMessage="*" Display="Static" ControlToValidate="email" /><ASP:RegularExpressionValidator id="REEmailValidator" ControlToValidate="email" runat="server" ErrorMessage="*" Display="Static" /> 
                              </td>
                              <td> </td>
                            </tr>
                            <tr> 
                              <td>&nbsp;</td>
                              <td align="left">
                              	<asp:button id="addressbookEntryUpdate" CausesValidation="true" runat="server" text="Update"  OnClick="AddressUpdate_Click" />
                              	<asp:button id="addressbookEntryInsert" CausesValidation="true" runat="server" text="Insert"  OnClick="AddressInsert_Click" />
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
