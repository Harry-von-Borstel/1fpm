using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controls
{
    public class MockFileSystem
    {
        public MockFileSystem()
        {
            this.Selected = new ShellMockFolder("Europe",
                                new ShellMockFolder("France"),
                                new ShellMockFolder("Poland"),
                                new ShellMockFolder("Germany"),
                                new ShellMockFolder("Sweden")
                            );

            this.Top =
                new ShellMockFolder("World",
                    new ShellMockFolder("Continents",
                        new ShellMockFolder("America",
                            new ShellMockFolder("USA"),
                            new ShellMockFolder("Canada"),
                            new ShellMockFolder("Brazil")
                        ),
                        this.Selected),
                    new ShellMockFolder("Oceans",
                        new ShellMockFolder("Pacific"),
                        new ShellMockFolder("Atlantic"),
                        new ShellMockFolder("Indian")
                        )
                    );
        }

        public ShellMockFolder Top { get; set; }

        public ShellMockFolder Selected { get; set; }
    }
}
