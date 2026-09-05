using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;
        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
            _conn.CreateTableAsync<Produto>().Wait();
        }

        public Task<int> Insert(Produto p)
        {
            return _conn.InsertAsync(p);
        }

        public Task<int> Update(Produto p)
        {
            string sql = "UPDATE Produto SET Descricao=?, Quantidade=?, Preco=?, Categoria=? WHERE Id=?";
            return _conn.ExecuteAsync(sql, p.Descricao, p.Quantidade, p.Preco, p.Categoria, p.Id);
        }

        public Task<int> Delete(int id)
        {
            return _conn.Table<Produto>().DeleteAsync(i => i.Id == id);
        }

        public Task<List<Produto>> GetAll()
        {
            return _conn.Table<Produto>().ToListAsync();
        }

        public Task<List<Produto>> Search(string q)
        {
            string sql = "SELECT * FROM Produto WHERE Descricao LIKE ?";
            return _conn.QueryAsync<Produto>(sql, "%" + q + "%");
        }

        // NOVO: busca produtos de uma categoria específica
        public Task<List<Produto>> GetByCategoria(string categoria)
        {
            string sql = "SELECT * FROM Produto WHERE Categoria = ?";
            return _conn.QueryAsync<Produto>(sql, categoria);
        }

        // NOVO: gera o relatório de total gasto por categoria
        public async Task<List<CategoriaTotal>> GetTotalPorCategoria()
        {
            var produtos = await _conn.Table<Produto>().ToListAsync();

            return produtos
                .GroupBy(p => string.IsNullOrWhiteSpace(p.Categoria) ? "Sem Categoria" : p.Categoria)
                .Select(g => new CategoriaTotal
                {
                    Categoria = g.Key,
                    QuantidadeItens = g.Count(),
                    Total = g.Sum(p => p.Total)
                })
                .OrderByDescending(c => c.Total)
                .ToList();
        }
    }
}