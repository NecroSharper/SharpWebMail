<%@ Page Language="C#"  Inherits="anmar.SharpWebMail.UI.Default" trace="false"%>
<%@ Register TagPrefix="SharpUI" TagName="globalUI" Src="globalUI.ascx" %>
<%@ Reference Page="search.aspx" %>
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
		<table><tr><td><img src="images/spacer.gif" width="1" height="3"></td></tr></table>
		<asp:DataGrid id="InboxDataGrid" runat="server"
			AllowPaging="True"
			PageSize="10"
			PagerStyle-HorizontalAlign="Right"
			OnPageIndexChanged="InboxDataGrid_PageIndexChanged"
			OnItemDataBound="InboxDataGrid_ItemDataBound"
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
			OnDeleteCommand="InboxDataGrid_Delete"
			AllowSorting="true"
			OnSortCommand="InboxDataGrid_Sort">
			<Columns>
				<asp:TemplateColumn>
					<HeaderTemplate>
						<asp:ImageButton id="SortButtoNumberUp" runat="server"
							CommandName="sort" CommandArgument="msgnum ASC"
							ImageUrl="images/sort_up.gif"
							/>
						<asp:Label id="inboxHeaderNumber" Text="<%# InboxDataGrid.Columns[0].HeaderText %>" runat="server" />
						<asp:ImageButton id="SortButtoNumberDown" runat="server"
							CommandName="sort" CommandArgument="msgnum DESC"
							ImageUrl="images/sort_down.gif"
							/>
					</HeaderTemplate>
					<ItemTemplate>
						<asp:Label runat="server"
							Text='<%# System.Web.HttpUtility.HtmlEncode (((System.Data.DataRowView)Container.DataItem)["msgnum"].ToString()) %>' />
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<HeaderTemplate>
						<asp:ImageButton id="SortButtonFromUp" runat="server"
							CommandName="sort" CommandArgument="FromName ASC"
							ImageUrl="images/sort_up.gif"
							/>
						<asp:Label id="inboxHeaderFrom" Text="<%# InboxDataGrid.Columns[1].HeaderText %>" runat="server" />
						<asp:ImageButton id="SortButtonFromDown" runat="server"
							CommandName="sort" CommandArgument="FromName DESC"
							ImageUrl="images/sort_down.gif"
							/>
					</HeaderTemplate>
					<ItemTemplate>
						<asp:HyperLink id="inboxItemFromLink" Text='<%# System.Web.HttpUtility.HtmlEncode (((System.Data.DataRowView)Container.DataItem)["FromName"].ToString()) %>' NavigateUrl='<%# System.String.Format("newmessage.aspx?msgid={0}", System.Web.HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "uidl", "{0:G}"))) %>' Target="_self" runat="server" />
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<HeaderTemplate>
						<asp:ImageButton id="SortButtonSubjectUp" runat="server"
							CommandName="sort" CommandArgument="Subject ASC"
							ImageUrl="images/sort_up.gif"
							/>
						<asp:Label id="inboxHeaderSubject" Text="<%# InboxDataGrid.Columns[2].HeaderText %>" runat="server" />
						<asp:ImageButton id="SortButtonSubjectDown" runat="server"
							CommandName="sort" CommandArgument="Subject DESC"
							ImageUrl="images/sort_down.gif"
							/>
					</HeaderTemplate>
					<ItemTemplate>
						<asp:HyperLink id="inboxItemSubjectLink" Text='<%# System.Web.HttpUtility.HtmlEncode (((System.Data.DataRowView)Container.DataItem)["Subject"].ToString()) %>' NavigateUrl='<%# System.String.Format("readmessage.aspx?msgid={0}", System.Web.HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "uidl", "{0:G}"))) %>' Target="_self" runat="server" />
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<HeaderTemplate>
						<asp:ImageButton id="SortButtonDateUp" runat="server"
							CommandName="sort" CommandArgument="Date ASC"
							ImageUrl="images/sort_up.gif"
							/>
						<asp:Label id="inboxHeaderDate" Text="<%# InboxDataGrid.Columns[3].HeaderText %>" runat="server" />
						<asp:ImageButton id="SortButtonDateDown" runat="server"
							CommandName="sort" CommandArgument="Date DESC"
							ImageUrl="images/sort_down.gif"
							/>
					</HeaderTemplate>
					<ItemTemplate>
						<asp:Label runat="server"
							Text='<%# System.Web.HttpUtility.HtmlEncode (((System.Data.DataRowView)Container.DataItem)["Date"].ToString()) %>' />
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<HeaderTemplate>
						<asp:ImageButton id="SortButtonSizeUp" runat="server"
							CommandName="sort" CommandArgument="size ASC"
							ImageUrl="images/sort_up.gif"
							/>
						<asp:Label id="inboxHeaderSize" Text="<%# InboxDataGrid.Columns[4].HeaderText %>" runat="server" />
						<asp:ImageButton id="SortButtonSizeDown" runat="server"
							CommandName="sort" CommandArgument="size DESC"
							ImageUrl="images/sort_down.gif"
							/>
					</HeaderTemplate>
					<ItemTemplate>
						<asp:Label runat="server"
							Text='<%# System.Web.HttpUtility.HtmlEncode (((System.Data.DataRowView)Container.DataItem)["size"].ToString()) %>' />
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<HeaderTemplate>
						<asp:ImageButton runat="server" ID="msgtoolbarDelete" ToolTip='<%# InboxDataGrid.Columns[5].HeaderText %>' ImageAlign="Middle" CommandName="delete"
                                         ImageUrl="images/msgtoolbar_delete.gif"/>
					</HeaderTemplate>
					<ItemTemplate>
						<input type="checkbox" title='<%# InboxDataGrid.Columns[5].HeaderText %>' OnServerChange="InboxDataGrid_DeleteCheckBox" ID="delete" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "uidl", "{0:G}")%>' />
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>
		</asp:DataGrid>
		<asp:PlaceHolder id="inboxWindowSearchHolder" runat="server" />
	</CentralPanel>
</SharpUI:globalUI>
</form>
</body>
</html>
