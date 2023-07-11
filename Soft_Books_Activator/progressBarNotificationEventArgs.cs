/*
 * Created by SharpDevelop.
 * User: "kevin mutugi, kevinmk30@gmail.com, +254717769329"
 * Date: 09/14/2018
 * Time: 16:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Soft_Books_Activator
{
	/// <summary>
	/// Description of progressBarNotificationEventArgs.
	/// </summary>
	public class progressBarNotificationEventArgs: EventArgs
	{
		public progressBarNotificationEventArgs(int _ProgressPercentage, int _ProgressMaximum){
			this.ProgressPercentage = _ProgressPercentage; 
			this.ProgressMaximum = _ProgressMaximum; 
		} 
		public int ProgressPercentage { get; private set; }  
		public int ProgressMaximum { get; private set; }  
		 
	}
}
