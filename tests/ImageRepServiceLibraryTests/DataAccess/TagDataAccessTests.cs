using ImageRepServiceLibrary.DataAccess;
using ImageRepServiceLibrary.Model;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace ImageRepServiceLibraryTests.DataAccess
{
    public class TagDataAccessTests
    {
        private readonly bool _commiteToDB = false;

        private ITagDataAccess Setup()
        {
            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock
                .SetupGet(m => m[It.Is<string>(s => s == "MainDatabase")])
                .Returns("Data Source=host.docker.internal,5050; Initial Catalog=ImageRep;User Id=sa;Password=P@assword123;MultipleActiveResultSets=true");

            var configurationMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            configurationMock
                .Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings")))
                .Returns(configurationSectionMock.Object);

            var recipeRepository = new TagDataAccess(configurationMock.Object);
            return recipeRepository;
        }

        [Fact]
        public async Task InsertSingleTag()
        {
            ITagDataAccess tagDataAccess = Setup();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                Guid id = Guid.NewGuid();
                try
                {
                    await tagDataAccess.InsertSingleTagAsync(new ImageRepServiceLibrary.Model.Tag()
                    {
                        TagId = id,
                        Name = "Doge"
                    });
                }
                catch (Exception e)
                {
                    Assert.NotNull(e);
                }

                try
                {
                    await tagDataAccess.InsertSingleTagAsync(new ImageRepServiceLibrary.Model.Tag()
                    {
                        TagId = id,
                        Name = ""
                    });
                }
                catch (TagDataException e)
                {
                    Assert.Equal("Tag name is not valid.", e.Message);
                }


                if (_commiteToDB)
                {
                    scope.Complete();
                }
            }
        }

        [Fact]
        public async Task DeleteTag()
        {
            IImageDataAccess imageDataAccess = ImageDataAccessTests.Setup();
            ITagDataAccess tagDataAccess = Setup();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var id = Guid.NewGuid();

                Image testImage = new Image()
                {
                    Id = id,
                    Name = "Dog Picture",
                    Description = "A picture of my dog in the field",
                    Picture = "asdfghjklqwertyuiop",
                    Tags = new List<Tag>()
                    {
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Dog",
                            ImageKey = id
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Field",
                            ImageKey = id
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Sky",
                            ImageKey = id
                        }
                    }
                };

                var rows = await imageDataAccess.SaveImageAsync(testImage);
                Assert.Equal(4, rows);

                int row = await tagDataAccess.DeleteTagAsync(testImage.Tags[0].TagId);
                Assert.Equal(1, row);

                Image fromDb = await imageDataAccess.GetImageAsync(id, true);
                Assert.Equal(2, fromDb.Tags.Count);
                Assert.False(fromDb.Tags.Contains(testImage.Tags[0]));
                Assert.True(fromDb.Tags.Contains(testImage.Tags[1]));
                Assert.True(fromDb.Tags.Contains(testImage.Tags[2]));



                row = await tagDataAccess.DeleteTagAsync(testImage.Tags[1].TagId);
                Assert.Equal(1, row);

                fromDb = await imageDataAccess.GetImageAsync(id, true);
                Assert.Equal(1, fromDb.Tags.Count);
                Assert.False(fromDb.Tags.Contains(testImage.Tags[0]));
                Assert.False(fromDb.Tags.Contains(testImage.Tags[1]));
                Assert.True(fromDb.Tags.Contains(testImage.Tags[2]));
                
                row = await tagDataAccess.DeleteTagAsync(testImage.Tags[2].TagId);
                Assert.Equal(1, row);

                fromDb = await imageDataAccess.GetImageAsync(id, true);
                Assert.Equal(0, fromDb.Tags.Count);

                if (_commiteToDB)
                    scope.Complete();
            }
        }
        
    }
}
