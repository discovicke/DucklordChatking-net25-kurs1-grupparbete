using ChatServer.Store;
using Xunit;

namespace Tests;

public class UserStoreTests
{
    [Fact]
    public void Add_NewUser_ShouldSucceed()
    {
        var store = new UserStore();

        bool added = store.Add("Ducklord", "chatking");

        Assert.True(added);
        Assert.NotNull(store.GetByUsername("Ducklord"));
    }

    [Fact]
    public void Add_ExistingUser_ShouldFail()
    {
        var store = new UserStore();
        store.Add("Ducklord", "chatking");

        bool addedAgain = store.Add("Ducklord", "otherpass");

        Assert.False(addedAgain);
    }
}