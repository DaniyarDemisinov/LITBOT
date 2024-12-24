using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LITBOT
{
    public class DapperBookRepository
    {
        private NpgsqlConnection _connection;
        private const string _connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=6p9xsyrqgc;Database=litbot";
        public DapperBookRepository()
        {
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();
        }
        public async Task InsertToBook(Book book)
        {
            string commandText = "INSERT INTO book " +
                "(genre, author, name, path_picture, path_book) " +
                "VALUES " +
                "(@genre, @author, @name, @path_picture, @path_book)";
            var queryArguments = new 
            {
                genre = book.genre,
                author = book.author,
                name = book.name,
                path_picture = book.pathPicture,
                path_book = book.pathBook
            };
            await _connection.ExecuteAsync(commandText, queryArguments);
        }
    }
}
