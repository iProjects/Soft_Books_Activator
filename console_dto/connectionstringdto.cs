/*
 * Created by: "kevin mutugi, kevinmk30@gmail.com, +254717769329"
 * Date: 01/23/2020
 * Time: 03:34
 */
using System;

namespace console_dto
{
	public abstract class connectionstringdto
	{ 
		public string datasource { get; set; }
		public string database { get; set; }
		public string userid { get; set; }
		public string password { get; set; }
		public string port { get; set; }
		public string new_database_name { get; set; }
		public string update_schema_temp_db { get; set; }
		public string update_schema_current_db { get; set; }
	}
	
}
