<%@ Page Language="C#"  Inherits="anmar.SharpWebMail.UI.Pages.ReadMessage" trace="false"%>
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
                              <td width="80%" align="left" class="XPPanel"><asp:Label id="newMessageWindowTitle" runat="server"/></td>
							  <td width="20%" align="right" class="XPPanel">
                                <table width="100%"  border="0">
                                  <tr>
								    <td width="100%"><img src="images/spacer.gif" width="1" height="1"></td>
								    <td nowrap><asp:ImageButton runat="server" ID="msgtoolbarReply" OnCommand="msgtoolbarCommand" CommandName="reply"
                                           ImageUrl="images/msgtoolbar_reply.gif"/>
									<img src="images/spacer.gif" width="3" height="14"></td>
									<td nowrap><asp:ImageButton runat="server" ID="msgtoolbarForward" OnCommand="msgtoolbarCommand" CommandName="forward"
                                           ImageUrl="images/msgtoolbar_forward.gif"/>
										   <img src="images/spacer.gif" width="3" height="14"></td>
								    <td><asp:HyperLink runat="server" ID="msgtoolbarHeader" NavigateUrl="javascript:void(0);"
                                           ImageUrl="images/msgtoolbar_header.gif"/><img src="images/spacer.gif" width="8" height="14"></td>
                                    <td>
                                      <asp:ImageButton runat="server" ID="msgtoolbarDelete" OnCommand="msgtoolbarCommand" CommandName="delete"
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
                          <table align="center" width="97%" cellpadding="0" cellspacing="0">
                            <tr> 
                              <td align="right" valign="top" width="1%" nowrap> <asp:Label id="newMessageWindowFromNameLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> <asp:Label id="readMessageWindowFromTextLabel" CssClass="XPFormText" runat="server"/>
                              </td>
                              <td> </td>
                            </tr>
                            <tr> 
                              <td align="right" valign="top" width="1%" nowrap> <asp:Label id="newMessageWindowDateLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> <asp:Label id="readMessageWindowDateTextLabel" CssClass="XPFormText" runat="server"/>
                              </td>
                              <td> </td>
                            </tr>
                            <tr> 
                              <td align="right" valign="top" nowrap> <asp:Label id="newMessageWindowToEmailLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> <asp:Label id="readMessageWindowToTextLabel" CssClass="XPFormText" runat="server"/>
                              </td>
                              <td> </td>
                            </tr>
                            <tr> 
                              <td align="right" valign="top" nowrap> <asp:Label id="newMessageWindowCcLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> <asp:Label id="readMessageWindowCcTextLabel" CssClass="XPFormText" runat="server"/>
                              </td>
                              <td> </td>
                            </tr>
                            <tr> 
                              <td align="right" valign="top" nowrap><asp:Label id="newMessageWindowSubjectLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> <asp:Label id="readMessageWindowSubjectTextLabel" CssClass="XPFormText" runat="server"/> 
                              </td>
                              <td> </td>
                            </tr>
                            <tr> 
                              <td align="right"><asp:Label id="readMessageWindowAttachmentsLabel" CssClass="XPFormLabel" runat="server"/></td>
                              <td> <asp:PlaceHolder id="readMessageWindowAttachmentsHolder" runat="server"/>
                              </td>
                              <td> </td>
                            </tr>
                            <tr> 
                              <td nowrap><img src="images/spacer.gif" width="1" height="6"></td>
                              <td></td>
                              <td></td>
                            </tr>
                            <tr> 
                              <td colspan="3" height="200" valign="top">
                                <table width="99%" align="center" cellpadding="2" cellspacing="0" bgcolor="#FFFFFF">
                                  <tr>
                                    <td width="1"><img src="images/spacer.gif" width="1" height="200">
                                    </td>
                                    <td valign="top">
                                      <asp:PlaceHolder id="readMessageWindowBodyTextHolder" runat="server"/>
                                    </td>
                                  </tr>
                                </table>
                              </td>
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
