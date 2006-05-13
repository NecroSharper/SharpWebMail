<%@ Page Language="C#"  Inherits="anmar.SharpWebMail.UI.AddressBook" trace="false" validateRequest="true" %>
<%@ Register TagPrefix="SharpUI" TagName="globalUI" Src="globalUI.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
<title><%=Application["product"]%></title>
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
<link rel="stylesheet" type="text/css" href="sharpwebmail.css">
</head>
<body bgcolor="#FFFFFF" text="#000000" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0">
<script type="text/javascript"><!--
	function select_all (checked, form) {
		for ( var i=0; i<form.elements.length; i++ ) {
			var item = form.elements[i];
			if ( item.type=='checkbox' && (item.name.lastIndexOf(':delete')>0 || item.name.lastIndexOf('$delete')>0) ) {
				if ( checked!=item.checked )
					item.checked=checked;
			}
		}
	}
//--></script>
<form id="sharpwebmailform" method="post" runat="server">
<SharpUI:globalUI id="SharpUI" runat="server">
<CentralPanel>
		<table><tr><td><img src="images/spacer.gif" width="1" height="3"></td></tr></table>
		<table width="100%"><tr><td>
			<asp:Label id="addressbookLabel" Visible="false" CssClass="XPFormLabel" runat="server"/><img src="images/spacer.gif" width="1" height="3">
			<select ID="addressbookselect" OnChange="this.form.submit();" OnServerChange="AddressBook_Changed" Visible="false" runat="server" /><img src="images/spacer.gif" width="6" height="1">
			<a runat="server" ID="addressbookEntryInsert" Visible="false" /><img src="images/spacer.gif" width="6" height="1">
			<a runat="server" ID="addressbookImportExport" Visible="false" />
		</td></tr></table>
		<asp:DataGrid id="AddressBookDataGrid" runat="server"
			AllowPaging="True"
			PageSize="10"
			PagerStyle-HorizontalAlign="Right"
			OnPageIndexChanged="AddressBookDataGrid_PageIndexChanged"
			Width="100%"			
			BorderColor="black"
			BorderWidth="1"
			GridLines="Both"
			CellPadding="3"
			CellSpacing="0"
			EnableViewState="true"
			HeaderStyle-CssClass="XPPanel"
			ItemStyle-CssClass="XPInboxItemA"
			AlternatingItemStyle-CssClass="XPInboxItemB"
			AutoGenerateColumns="false"
			OnDeleteCommand="AddressBookDataGrid_Delete"
			AllowSorting="true"
			OnSortCommand="AddressBookDataGrid_Sort">
			<Columns>
				<asp:TemplateColumn>
					<HeaderTemplate>
						<asp:ImageButton id="SortButtonNameUp" runat="server"
							CommandName="sort" CommandArgument="[NameColumn] ASC"
							ImageUrl="images/sort_up.gif"
							/>
						<asp:Label id="addressbookNameLabel" runat="server" />
						<asp:ImageButton id="SortButtonNameDown" runat="server"
							CommandName="sort" CommandArgument="[NameColumn] DESC"
							ImageUrl="images/sort_down.gif"
							/>
					</HeaderTemplate>
					<ItemTemplate>
						<asp:Label runat="server"
							Text='<%# System.Web.HttpUtility.HtmlEncode (((System.Data.DataRowView)Container.DataItem)[0].ToString()) %>' />
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<HeaderTemplate>
						<asp:ImageButton id="SortButtonEMailUp" runat="server"
							CommandName="sort" CommandArgument="[EmailColumn] ASC"
							ImageUrl="images/sort_up.gif"
							/>
						<asp:Label id="addressbookEmailLabel" runat="server" />
						<asp:ImageButton id="SortButtonEMailDown" runat="server"
							CommandName="sort" CommandArgument="[EmailColumn] DESC"
							ImageUrl="images/sort_down.gif"
							/>
					</HeaderTemplate>
					<ItemTemplate>
						<a href="newmessage.aspx?to=<%# System.Web.HttpUtility.UrlEncode (((System.Data.DataRowView)Container.DataItem)[1].ToString()) %>">
							<asp:Label runat="server"
							Text='<%# System.Web.HttpUtility.HtmlEncode (((System.Data.DataRowView)Container.DataItem)[1].ToString()) %>' />
						</a>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<ItemTemplate>
						<a href="addressbook_edit.aspx?addr=<%# System.Web.HttpUtility.UrlEncode (((System.Data.DataRowView)Container.DataItem)[1].ToString()) %>&amp;book=<%#System.Web.HttpUtility.UrlEncode(((System.Data.DataRowView)Container.DataItem)[2].ToString()) %>">
							<asp:Label runat="server" ID="addressbookEntryEdit" />
						</a>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<HeaderTemplate>
						<asp:ImageButton runat="server" ID="msgtoolbarDelete" ImageAlign="Baseline" CommandName="delete"
                                         ImageUrl="images/msgtoolbar_delete.gif"/>
						<input type="checkbox" onclick="select_all(this.checked, this.form);" value="" />
					</HeaderTemplate>
					<ItemTemplate>
						<input type="checkbox" OnServerChange="AddressBookDataGrid_DeleteCheckBox" ID="delete" runat="server" value='<%#((System.Data.DataRowView)Container.DataItem)[1]%>' />
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>
		</asp:DataGrid>
	</CentralPanel>
</SharpUI:globalUI>
</form>
</body>
</html>
