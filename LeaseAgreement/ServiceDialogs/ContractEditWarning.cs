using System;
using LeaseAgreement.Domain;
using Gtk;
using System.Collections.Generic;

namespace LeaseAgreement
{
	public partial class ContractEditWarning : Gtk.Dialog
	{
		public ContractEditWarning (IList<ContractPlace> places, String message)
		{
			this.Build ();
			this.Title = "";
			label1.Text = message;
			ytreeviewPlaces.ItemsDataSource = places;
			ytreeviewPlaces.Selection.Mode = Gtk.SelectionMode.None;
			ytreeviewPlaces.ColumnsConfig = Gamma.GtkWidgets.ColumnsConfigFactory.Create<ContractPlace> ()
				.AddColumn ("Место").AddTextRenderer (node => node.Place.Title)
				.AddColumn ("Начало аренды")
				.AddTextRenderer (node => node.StartDate.Value.ToShortDateString ())
				.AddColumn ("Окончание аренды")
				.AddTextRenderer (node => node.EndDate.Value.ToShortDateString ())
				.RowCells ().AddSetter<Gtk.CellRendererText> ((c, n) => c.Foreground = n.RowColor)
				.Finish ();
				
			buttonOk.Visible = true;
		}

		public ContractEditWarning (IList<DateChange> changes, String message)
		{
			this.Build ();
			this.Title = "Изменения в договоре";
			label1.Text = message;
			ytreeviewPlaces.ItemsDataSource = changes;
			ytreeviewPlaces.Selection.Mode = Gtk.SelectionMode.None;
			ytreeviewPlaces.ColumnsConfig = Gamma.GtkWidgets.ColumnsConfigFactory.Create<DateChange> ()
				.AddColumn ("Место").AddTextRenderer (node => node.contractPlace.Place.Title)
				.AddColumn ("Начало аренды")
				.AddTextRenderer (node => node.ToStartDateString ())
				.AddColumn ("Окончание аренды")
				.AddTextRenderer (node => node.ToEndDateString ())
				.RowCells ().AddSetter<Gtk.CellRendererText> ((c, n) => c.Foreground = n.contractPlace.RowColor)
				.Finish ();
			buttonOk.Visible = true;
			buttonCancel.Visible = true;
		}
	}
}
public class DateChange
{
	public ContractPlace contractPlace;
	public DateTime newStartDate;
	public DateTime newEndDate;

	public DateChange (ContractPlace contractPlace, DateTime newStartDate, DateTime newEndDate)
	{
		this.contractPlace = contractPlace;
		this.newStartDate = newStartDate;
		this.newEndDate = newEndDate;
	}

	public void apply ()
	{		
		contractPlace.StartDate = newStartDate;
		contractPlace.EndDate = newEndDate;
	}

	public String ToStartDateString ()
	{		
		return (contractPlace.StartDate == newStartDate) ? contractPlace.StartDate.Value.ToShortDateString () 
				: contractPlace.StartDate.Value.ToShortDateString () + " -> "
		+ newStartDate.ToShortDateString ();
	}

	public String ToEndDateString ()
	{		
		return (contractPlace.EndDate == newEndDate) ? contractPlace.EndDate.Value.ToShortDateString () 
				: contractPlace.EndDate.Value.ToShortDateString () + " -> "
		+ newEndDate.ToShortDateString ();
	}
}

