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
        public async Task InsertToGenre(Genre genre)
        {
            string commandText = 
                "INSERT INTO genre (name) VALUES (@name)" +
                "ON CONFLICT (name) " +
                "DO NOTHING";
            var queryArguments = new
            {
                name = genre.name
            };
            await _connection.ExecuteAsync(commandText, queryArguments);
        }
        public int GetLastIdTable(string tableName)
        {
            string text = 
                $"SELECT id FROM {tableName} " +
                $"ORDER BY id DESC";
            
            var result = _connection.QueryFirstOrDefault<int>(text);
            return result;
        }
        public async Task InsertToAuthor(Author author, int genreId)
        {
            string commandText = "INSERT INTO author " +
                "(name, genre_id) " +
                "VALUES " +
                "(@name, @genre_id) " +
                "ON CONFLICT (name) " +
                "DO NOTHING";
            var queryArguments = new
            {
                name = author.name,
                genre_id = genreId
            };
            await _connection.ExecuteAsync(commandText, queryArguments);
        }
        //public async Task<int> GetLastId(string tableName)
        //{
        //    string text =
        //        $"SELECT " +
        //        $"id " +
        //        $"FROM {tableName} " +
        //        $"ORDER BY id DESC " +
        //        $"LIMIT @a";
        //    var queryArguments = new
        //    {
        //        a = 1
        //    };
        //    var result = await _connection.ExecuteAsync(text, queryArguments);
        //    return result;
        //}

        public async Task InsertToBook(Book book, int genreId, int authorId)
        {
            string commandText = "INSERT INTO book " +
                "(name, picture_path, book_path, genre_id, author_id) " +
                "VALUES " +
                "(@name, @picture_path, @book_path, @genre_id, @author_id) " +
                "ON CONFLICT (name) " +
                "DO NOTHING";
            var queryArguments = new 
            {
                name = book.name,
                picture_path = book.picturePath,
                book_path = book.bookPath,
                genre_id = genreId,
                author_id = authorId
            };
            await _connection.ExecuteAsync(commandText, queryArguments);
        }
        
    }
}
