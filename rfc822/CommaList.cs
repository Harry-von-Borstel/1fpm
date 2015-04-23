using System.Linq;

namespace blueshell.rfc822
{
    public class CommaList<O>
         : ArgumentBase
       where O: ArgumentBase, new()
    {
        private readonly O[] list;

    	public CommaList(params O[] items)
        {
            list = items;
        }

        public CommaList(string commaListString)
        {
            var array =commaListString.Split(',');
            this.list = new O[array.Length];
            for (int i = 0; i < array.Length;i++ )
            {
                var o = new O();
                o.Set(array[i]);
                this.list[i] = o;
            }
        }

        public override string ToString()
        {
            return list.Aggregate(
                "",
                (total, next) =>
                    string.Format(
                        "{0}{2}{1}",
                        total,
                        next.ToString(),
                        total == ""
                            ? ""
                            : ",")
                );
        }

        public static implicit operator   CommaList<O>(O singleItem)
        {
            return new CommaList<O>(singleItem);
        }
    }
}
