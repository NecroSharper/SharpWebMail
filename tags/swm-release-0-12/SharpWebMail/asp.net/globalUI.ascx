<%@ Control Language="C#"  Inherits="anmar.SharpWebMail.UI.globalUI" %>
<table width="100%" cellpadding="0" cellspacing="0" align="center">
  <tr> 
    <td colspan="3"> 
      <table cellpadding="0" cellspacing="0" width="100%">
        <tr> 
          <td class="XPTitleBarLeft"><img src="images/spacer.gif" width="6" height="30"></td>
          <td class="XPTitleBar"><img src="images/spacer.gif" width="10" height="1"></td>
          <td class="XPTitleBar" width="100%"><asp:Label EnableViewState="false" id="defaultWindowTitle" runat="server"></asp:Label></td>
          <td class="XPTitleBar"></td>
          <td class="XPTitleBarClose"><img src="images/spacer.gif" width="21" height="30"></td>
          <td class="XPTitleBarRight"><img src="images/spacer.gif" width="6" height="30"></td>
        </tr>
      </table>
    </td>
  </tr>
  <tr> 
    <td class="XPBorderLeft"></td>
    <td width="100%"> 
      <table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr> 
          <td colspan="2" class="XPToolbar">
            <table width="100%" border="0" cellspacing="0" cellpadding="0">
              <tr>
                <td class="XPToolbar">
                  <table width="100%" border="0" cellspacing="0" cellpadding="0">
                    <tr> 
                      <td width="20"><input type="image" name="spacer" src="images/spacer.gif" border="0" /></td>
                      <td width="40"><asp:ImageButton id="logoutImageButton" runat="server"
							onClick="logOutSessionButton_Click"
							CausesValidation="false"
							EnableViewState="false"
							ImageUrl="images/navbar_button_logout.gif"
							/>
                      </td>
                      <td width="32"><asp:ImageButton id="prevPageImageButton" runat="server"
							CausesValidation="false"
							EnableViewState="false"
							ImageUrl="images/navbar_button_back.gif"
							/>
					  </td>
                      <td width="40"><asp:ImageButton id="nextPageImageButton" runat="server"
							CausesValidation="false"
							EnableViewState="false"
							ImageUrl="images/navbar_button_forward.gif"
							/>
					  </td>
                      <td><asp:ImageButton id="refreshPageImageButton" runat="server"
							CausesValidation="false"
							EnableViewState="false"
							ImageUrl="images/navbar_button_refresh.gif"
							/>
                      </td>
                      <td>&nbsp;</td>
                    </tr>
                  </table>
                </td>
                <td align="right" class="XPToolbar"><asp:Label EnableViewState="false" id="messageCountLabel"  runat="server"></asp:Label>&nbsp;</td>
              </tr>
            </table>
          </td>
        </tr>
        <tr> 
          <td width="25%" valign="top" height="100%"> 
            <table width="90%" border="0" cellspacing="0" cellpadding="0" align="left" height="100%">
              <tr> 
                <td class="XPPanel"></td>
                <td class="XPPanelBorder" rowspan="3" width="6"><img src="images/spacer.gif" width="6" height="250"></td>
              </tr>
              <tr> 
                <td valign="top"> 
                  <table width="100%" border="0" cellspacing="0" cellpadding="0" align="right">
                    <tr> 
                      <td class="XPPanel" colspan="2" align="right">
                        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="XPPanelTitle">
                          <tr>
                            <td width="100%" class="XPPanel"><asp:Label EnableViewState="false" id="optionsLabel"  runat="server"></asp:Label></td>
                            <td align="right" nowrap><img src="images/PanelClose.gif" width="11" height="11"><img src="images/spacer.gif" width="3" height="1"></td>
                          </tr>
                        </table>
                        </td>
                    </tr>
                    <tr>
                      <td class="XPPanel" colspan="2" align="right"><img src="images/spacer.gif" width="1" height="4"></td>
                    </tr>
                    <tr> 
                      <td colspan="2" align="right"><img src="images/spacer.gif" width="20" height="5"></td>
                    </tr>
                    <tr> 
                      <td><img src="images/spacer.gif" width="20" height="1"></td>
                      <td nowrap valign="middle"><asp:LinkButton id="inboxLinkButton" EnableViewState="false" CssClass="XPFormLabel" onClick="inboxLinkButton_Click" CausesValidation="false" runat="server" /></td>
                    </tr>
                    <tr> 
                      <td></td>
                      <td nowrap valign="middle"><asp:LinkButton id="trashLinkButton" EnableViewState="false" CssClass="XPFormLabel" onClick="trashLinkButton_Click" CausesValidation="false" runat="server" /></td>
                    </tr>
                    <tr> 
                      <td></td>
                      <td nowrap valign="middle"><asp:LinkButton id="newMessageLinkButton" EnableViewState="false" CssClass="XPFormLabel" onClick="newMessageLinkButton_Click" CausesValidation="false" runat="server" /></td>
                    </tr>
                    <tr> 
                      <td></td>
                      <td nowrap valign="middle"><asp:LinkButton id="searchLinkButton" EnableViewState="false" CssClass="XPFormLabel" onClick="searchLinkButton_Click" CausesValidation="false" runat="server" /></td>
                    </tr>
                    <tr> 
                      <td></td>
                      <td nowrap valign="middle"><asp:LinkButton id="logoutLinkButton" EnableViewState="false" CssClass="XPFormLabel" onClick="logoutLinkButton_Click" CausesValidation="false" runat="server" /></td>
                    </tr>
                  </table>
                </td>
              </tr>
              <tr> 
                <td height="90%"><img src="images/spacer.gif" width="10" height="10"></td>
              </tr>
              <tr>
                <td class="XPPanelFooter"><img src="images/spacer.gif" width="1" height="2"></td>
				<td class="XPPanelFooterRight"><img src="images/spacer.gif" width="6" height="6"></td>
              </tr>
            </table>
          </td>
          <td valign="top"> 
            <table width="98%" border="0" cellspacing="0" cellpadding="0">
              <tr>
                <td><img src="images/spacer.gif" width="3" height="1"></td>
                <td valign="middle" align="center"><asp:PlaceHolder id="centralPanelHolder" runat="server"/></td>
                <td><img src="images/spacer.gif" width="3" height="1"></td>
              </tr>
            </table>
          </td>
        </tr>
        <tr> 
          <td colspan="2" class="XPToolbar"><a href="http://anmar.eu.org/projects/sharpwebmail/" class="XPToolbarFooter" target="_blank"><%=Application["product"]%> - v<%=Application["version"]%></a> - Copyright &copy; 2003-2005 <a href="http://anmar.eu.org" class="XPToolbarFooter" target="_blank"> 
            Angel Marin</a> All rights reserved.</td>
        </tr>
      </table>
    </td>
    <td class="XPBorderRight"></td>
  </tr>
  <tr> 
    <td class="XPFooterLeft"><img src="images/spacer.gif" width="4" height="4"></td>
    <td class="XPFooter"></td>
    <td class="XPFooterLeft"><img src="images/spacer.gif" width="4" height="4"></td>
  </tr>
</table>


