using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class RelatorioCategoria : ContentPage
{
    ObservableCollection<CategoriaTotal> dados = new ObservableCollection<CategoriaTotal>();

    public RelatorioCategoria()
    {
        InitializeComponent();
        lst_relatorio.ItemsSource = dados;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarRelatorio();
    }

    private async Task CarregarRelatorio()
    {
        try
        {
            var totais = await App.Db.GetTotalPorCategoria();

            dados.Clear();
            foreach (var t in totais)
                dados.Add(t);

            double totalGeral = totais.Sum(t => t.Total);
            lbl_total_geral.Text = $"Total Geral: {totalGeral:C}";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_relatorio_Refreshing(object sender, EventArgs e)
    {
        await CarregarRelatorio();
        lst_relatorio.IsRefreshing = false;
    }
}