using System.Collections.Generic;

namespace Abp.Application.Navigation
{
    public class UserNavigationNode
    {
        private readonly List<UserNavigationNode> _list = new List<UserNavigationNode>();
        public string Name { get; set; }
        public string ParentName { get; set; }
        public string DisplayName { get; set; }
        public int Order { get; set; }
        public bool IsActived { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public object CustomData { get; set; }
        public IList<UserNavigationNode> Children
        {
            get { return _list; }
        }

        public UserNavigationNode()
        {

        }

        public UserNavigationNode(UserMenu userMenu)
        {
            DisplayName = userMenu.DisplayName;
            CustomData = userMenu.CustomData;
            Name = userMenu.Name;
        }
        public UserNavigationNode(UserMenuItem userMenuItem)
        {
            DisplayName = userMenuItem.DisplayName;
            CustomData = userMenuItem.CustomData;
            Name = userMenuItem.Name;
            Icon = userMenuItem.Icon;
            Order = userMenuItem.Order;
            Url = userMenuItem.Url;
        }

        public UserNavigationNode Convert(UserMenu menu)
        {
            var node = new UserNavigationNode
            {
                DisplayName = menu.DisplayName,
                CustomData = menu.CustomData,
                Name = menu.Name
            };

            return node;
        }

        public UserNavigationNode Convert(UserMenuItem menu)
        {
            var node = new UserNavigationNode
            {
                DisplayName = menu.DisplayName,
                CustomData = menu.CustomData,
                Name = menu.Name,
                Icon = menu.Icon,
                Order = menu.Order,
                Url = menu.Url
            };

            return node;
        }
    }

    //public class UserNavigationNode<TCustom>
    //{
    //    private readonly List<UserNavigationNode<TCustom>> _list = new List<UserNavigationNode<TCustom>>();
    //    public string Name { get; set; }
    //    public string ParentName { get; set; }
    //    public string DisplayName { get; set; }
    //    public int Order { get; set; }
    //    public bool IsActived { get; set; }
    //    public string Icon { get; set; }
    //    public string Url { get; set; }

    //    public object CustomData { get; set; }
    //    public TCustom CustomNodeData { get; set; }

    //    public IList<UserNavigationNode<TCustom>> Children
    //    {
    //        get { return _list; }
    //    }

    //    public UserNavigationNode<TCustom> Convert(UserMenu menu)
    //    {
    //        var node = new UserNavigationNode<TCustom>();
    //        node.DisplayName = menu.DisplayName;
    //        node.CustomData =  menu.CustomData;
    //        node.CustomNodeData = menu;
    //    }
    //}
}
