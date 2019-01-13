using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public class DesignList : List<FormDesign>
	{
		public FormDesign this[string name] => this.FirstThat(x => x.Name == name) ?? FormDesign.Modern;

		public new FormDesign this[int id] => this.FirstThat(x => x.ID == id) ?? FormDesign.Modern;
	}
}
