using Choroszcz.ApiObjects;

namespace Choroszcz
{
    public static class TreePrinter
    {
        private const string _cross = " ├─";
        private const string _corner = " └─";
        private const string _vertical = " │ ";
        private const string _space = "   ";
        
        public static void Print(this ShoperCategoryTree[] tree, bool sort = true)
        {
            tree = sort ? tree.OrderBy(node => node.Id).ToArray() : tree;
            foreach (ShoperCategoryTree node in tree)
                PrintNode(node, sort);
        }
        static void PrintNode(ShoperCategoryTree node, bool sort, string indent = "", ConsoleColor color = ConsoleColor.Blue)
        {
            Console.ForegroundColor = color;
            Console.Write($"{node.Name} ({node.Id})");
            Console.ResetColor();
            Console.WriteLine();
            var child = sort ? node.Children.OrderBy(node => node.Id).ToArray() : node.Children;
            for (var i = 0; i < child.Length; i++)
                PrintChildNode(child[i], sort, indent, i == (child.Length - 1));
        }

        static void PrintChildNode(ShoperCategoryTree node, bool sort, string indent, bool last)
        {
            Console.Write(indent);
       
            if (last)
            {
                Console.Write(_corner);
                indent += _space;
            }
            else
            {
                Console.Write(_cross);
                indent += _vertical;
            }
            
            PrintNode(node, sort, indent, ConsoleColor.Green);
        }
    }
}
