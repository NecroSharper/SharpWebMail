<%@ Page Language="C#"  Inherits="anmar.SharpWebMail.UI.newmessage" trace="false" validateRequest="false" %>
<%@ Register TagPrefix="FredCK" Namespace="FredCK" Assembly="FredCK.FCKeditor" %>
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
                              <td width="100%" class="XPPanel"><asp:Label id="newMessageWindowTitle" runat="server"/></td>
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
                        <td width="100%" class="XPWindowBackground"> <asp:panel id="newMessagePanel" runat="server"> 
                          <table align="center" width="97%">
                            <tr> 
                              <td align="right" nowrap width="1%"> <asp:Label id="newMessageWindowFromNameLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> 
                                <input id="fromname" type="text" runat="server" value="" style="XPInput" name="text" />
								<ASP:RequiredFieldValidator id="ReqfromnameValidator" runat="server" ErrorMessage="*" Display="Static" ControlToValidate="fromname"></ASP:RequiredFieldValidator> 
                                <ASP:RegularExpressionValidator id="fromnameValidator" ValidationExpression="^\w[A-z0-9 ]+\w$" ControlToValidate="fromname" runat="server" ErrorMessage="*" Display="Static"></ASP:RegularExpressionValidator> 
                              </td>
                              <td> </td>
                            </tr>
                            <tr> 
                              <td align="right" nowrap> <asp:Label id="newMessageWindowFromEmailLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> <asp:Label id="email" CssClass="XPFormLabel" runat="server"/> 
                              </td>
                              <td></td>
                            </tr>
                            <tr> 
                              <td align="right" nowrap> <asp:Label id="newMessageWindowToEmailLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> 
                                <input id="toemail" type="text" runat="server" value="" style="XPInput" name="text" />
								<ASP:RequiredFieldValidator id="ReqemailValidator" runat="server" ErrorMessage="*" Display="Static" ControlToValidate="toemail"></ASP:RequiredFieldValidator> 
                                <ASP:RegularExpressionValidator id="toemailValidator" ValidationExpression="^^[A-z0-9_\-]+[@][A-z0-9_\-]+([.][A-z0-9_\-]+){0,}[.][A-z]{2,4}$" ControlToValidate="toemail" runat="server" ErrorMessage="*" Display="Static"></ASP:RegularExpressionValidator> 
                              </td>
                              <td> </td>
                            </tr>
                            <tr> 
                              <td align="right" nowrap><asp:Label id="newMessageWindowSubjectLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> 
                                <input id="subject" type="text" runat="server" value="" style="XPInput" name="text" />
								<ASP:RequiredFieldValidator id="ReqsubjectValidator" runat="server" ErrorMessage="*" Display="Static" ControlToValidate="subject"></ASP:RequiredFieldValidator> 
                                <ASP:RegularExpressionValidator id="subjectValidator" ValidationExpression="^[^ '][^']+[^ ]$" ControlToValidate="subject" runat="server" ErrorMessage="*" Display="Static"></ASP:RegularExpressionValidator> 
                              </td>
                              <td> </td>
                            </tr>
                            <tr> 
                              <td nowrap><asp:button id="newMessageWindowAttachmentsButton" causesvalidation="false" onClick="Attachments_Click" runat="server" text="Send"></asp:button></td>
                              <td>
                              <asp:DataList id="newMessageWindowAttachmentsAddedList"
                                   CellPadding="0"
                                   CellSpacing="2"
                                   RepeatDirection="Vertical"
                                   RepeatLayout="Flow"
                                   RepeatColumns="3"
                                   ShowBorder="false"
                                   runat="server">
								   <ItemTemplate>
								      <asp:Label id="Attachment" CssClass="XPDownload" Text='<%# DataBinder.Eval(Container.DataItem, "Text")%>' runat="server"/>
								   </ItemTemplate>
                              </asp:DataList>
							  </td>
                              <td>&nbsp;</td>
                            </tr>
                            <tr> 
                              <td colspan="2" height="200">
                                  <FredCK:FCKeditor id="FCKEditor" height="210" runat="server" BasePath="fcke/"></FredCK:FCKeditor>
                              </td>
                              <td></td>
                            </tr>
                            <tr> 
                              <td colspan="2" align="center"><asp:button id="newMessageWindowSendButton" onClick="Send_Click" runat="server" text="Send"></asp:button> 
                              </td>
                              <td>&nbsp;</td>
                            </tr>
                          </table>
                          </asp:panel> <asp:panel id="confirmationPanel" runat="server"> 
                          <table align="center">
                            <tr> 
                              <td></td>
                              <td align="center"> <asp:Label id="newMessageWindowConfirmation" CssClass="XPFormLabel" runat="server"/> 
                              </td>
                              <td> </td>
                            </tr>
                          </table>
                          </asp:panel>
                          </asp:panel> <asp:panel id="attachmentsPanel" runat="server"> 
                          <table align="center">
                            <tr> 
                              <td></td>
                              <td nowrap align="left"><asp:CheckBoxList id="newMessageWindowAttachmentsList" RepeatColumns="2" runat="server" CssClass="XPDownload"></asp:CheckBoxList>
                              </td>
                              <td> </td>
                            </tr>
                            <tr> 
                              <td colspan="3"><img src="images/spacer.gif" width="1" height="5"></td>
                            </tr>
                            <tr> 
                              <td></td>
                              <td align="center"><INPUT id="newMessageWindowAttachFile" type="file" size="20" runat="server">
                              </td>
                              <td> </td>
                            </tr>
                            <tr> 
                              <td></td>
                              <td align="center"><asp:button id="newMessageWindowAttachmentAddButton" onClick="AttachmentAdd_Click" runat="server" Text="Add"></asp:button>
                              </td>
                              <td> </td>
                            </tr>
                            <tr> 
                              <td colspan="3"><img src="images/spacer.gif" width="1" height="5"></td>
                            </tr>
                            <tr> 
                              <td></td>
                              <td align="center"><asp:button id="newMessageWindowAttachButton" causesvalidation="false" onClick="Attach_Click" runat="server" text="Attach"></asp:button>
                              </td>
                              <td> </td>
                            </tr>
                          </table>
                          </asp:panel></td>
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