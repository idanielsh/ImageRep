using ImageRepServiceLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace ImageRepServiceLibrary.DataAccess
{
    public interface IImageDataAccess
    {
        public Task<int> SaveImageAsync(Image image);
        public Task<Image> GetImageAsync(Guid guid, bool loadTags);
        public Task<IEnumerable<Image>> SearchImagesAsync(bool loadTags, string name = null, string description = null, string tagName = null);
        public Task<int> DeleteImageAsync(Guid guid);
        public Task<int> UpdateImageAsync(Image image, bool updateTags);
    }
    public class ImageDataAccess : IImageDataAccess
    {
        private readonly ITagDataAccess _tagDataAccess;
        private readonly string _connectionString;
        public ImageDataAccess(ITagDataAccess tagDataAccess, IConfiguration configuration)
        {
            _tagDataAccess = tagDataAccess;
            _connectionString = configuration.GetConnectionString("MainDatabase");
        }

        public async Task<int> DeleteImageAsync(Guid guid)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    int affectedRows = 0;
                    affectedRows += await _tagDataAccess.DeleteTagsByImageAsync(guid, transaction, connection);

                    affectedRows += await connection.ExecuteAsync(@"
                        DELETE FROM [dbo].[Images]
                            WHERE [Id] = @Id
                    ",
                    new
                    {
                        Id = guid
                    }, transaction: transaction);

                    transaction.Commit();
                    return affectedRows;
                }
            }
        }

        public async Task<IEnumerable<Image>> SearchImagesAsync(bool loadTags, string name = null, string description = null, string tagName = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    name = name == null? null: $"%{name}%";
                    description = description == null? null: $"%{description}%";

                    var images = await connection.QueryAsync<Image>(@"SELECT * FROM [dbo].[Images]
                          WHERE (@name IS null OR
								[Name] LIKE @name) AND
	                      (@description IS null OR
								[Description] LIKE @description) AND 
                          (@tagName IS null OR
								EXISTS 
								(SELECT * FROM [dbo].[Tags] WHERE 
										[Tags].Name = @tagName AND 
										[Images].[Id] = [Tags].ImageKey))
                            ORDER BY DateCreated DESC",
                        new
                        {
                            name,
                            description,
                            tagName

                        }, transaction: transaction);

                    if (loadTags)
                    {
                        foreach (var item in images)
                        {
                            item.Tags = (await _tagDataAccess.GetTagsForImageAsync(item.Id, transaction, connection)).ToList();
                        }
                    }
                    transaction.Commit();

                    return images;
                }
            }
        }

        public async Task<Image> GetImageAsync(Guid guid, bool loadTags)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    var images = (await connection.QueryAsync<Image>(@"select * FROM [dbo].[Images]
                        WHERE [Id] = @Id ",
                        new
                        {
                            Id = guid
                        }, transaction: transaction));

                    if (images.Count() == 0)
                    {
                        return null;
                    }
                    var image = images.First();

                    if(loadTags)
                        image.Tags = (await _tagDataAccess.GetTagsForImageAsync(guid, transaction, connection)).ToList();

                    transaction.Commit();

                    return image;

                }
            }
        }

        public async Task<int> SaveImageAsync(Image image)
        {
            ImageDataVerification.VerifyImageParameteres(image);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    var affectedRows = await connection.ExecuteAsync(@"
                    INSERT INTO [dbo].[Images]
                               ([Id]
                               ,[Name]
                               ,[Description]
                               ,[Picture]
                               ,[DateCreated])
                         VALUES
                               (@Id
                               ,@Name
                               ,@Description
                               ,@Picture
                               ,@DateCreated)",
                    new
                    {
                        image.Id,
                        image.Name,
                        image.Description,
                        image.Picture,
                        image.DateCreated
                    }, transaction: transaction);

                    affectedRows += await _tagDataAccess.InsertTagsAsync(image.Tags, transaction, connection);

                    transaction.Commit();

                    return affectedRows;
                }
            }
        }

        public async Task<int> UpdateImageAsync(Image image, bool updateTags)
        {
            ImageDataVerification.VerifyImageParameteres(image);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    var affectedRows = await connection.ExecuteAsync(@"UPDATE [dbo].[Images]
                        SET [Name] = @name,
                            [Description] = @description,
                            [Picture] = @picture,
                            [DateCreated] = @dateCreated
                        WHERE [Id] = @id",
                         new 
                         {
                             name = image.Name,
                             description = image.Description,
                             picture = image.Picture,
                             dateCreated = image.DateCreated,
                             id = image.Id
                         }, transaction: transaction);

                    if (updateTags)
                    {
                        affectedRows += await _tagDataAccess.DeleteTagsByImageAsync(image.Id, transaction, connection);
                        affectedRows += await _tagDataAccess.InsertTagsAsync(image.Tags, transaction, connection);   
                    }

                    transaction.Commit();

                    return affectedRows;
                }
            }
        }

        private static class ImageDataVerification
        {
            public static void VerifyImageParameteres(Image image)
            {
                if(image.Name == null || image.Name == "")
                {
                    throw new ImageDataException("Image name is not valid.");
                }
                if(image.Id == null)
                {
                    throw new ImageDataException("Image Id is not valid.");
                }
                if(image.Picture == null || image.Picture == "")
                {
                    throw new ImageDataException("Please upload an image.");
                }
            }
        }
    }

    public class ImageDataException : DBException
    { 
        public ImageDataException()
        {}

        public ImageDataException(string message)
            : base(message)
        {}

        public ImageDataException(string message, Exception inner)
            : base(message, inner)
        {}
    }

 }

