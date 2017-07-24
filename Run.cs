using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;

public class Run {
	
	[DllImport("kernel32.dll")]
	static extern bool FreeConsole();
	
	private static Battery battery;
	
	private static int interval = 30; // seconds
	private static bool console = false;
	
	public static void Main(string[] args) {
		battery = new Battery();
		ParseArgs(args);
		battery.UpdateBattery();
		
		System.Timers.Timer timer = new System.Timers.Timer(interval * 1000);
		timer.Elapsed += OnTimedEvent;
		timer.AutoReset = true;
		timer.Enabled = true;
		
		SystemEvents.PowerModeChanged += OnPowerChanged;
		
		if(!console) FreeConsole();
		Application.Run();
	}
	
	public static void ParseArgs(string[] args) {
		try {
			for(int i = 0; i < args.Length; i++) {				
				switch(args[i]) {
					case "--color-outline":
						if(++i < args.Length) battery.ColorOutline = ColorTranslator.FromHtml(args[i]);
						else throw new Exception("Outline color not supplied");
						break;
					case "--color-background":
						if(++i < args.Length) battery.ColorBackground = ColorTranslator.FromHtml(args[i]);
						else throw new Exception("Background color not supplied");
						break;
					case "--color-normal-charge":
						if(++i < args.Length) battery.ColorNormalCharge = ColorTranslator.FromHtml(args[i]);
						else throw new Exception("Normal charge color not supplied");
						break;
					case "--color-powersave-charge":
						if(++i < args.Length) battery.ColorPowerSaveCharge = ColorTranslator.FromHtml(args[i]);
						else throw new Exception("PowerSave color not supplied");
						break;
					case "--color-low-charge":
						if(++i < args.Length) battery.ColorLowCharge = ColorTranslator.FromHtml(args[i]);
						else throw new Exception("Low charge color not supplied");
						break;
					case "--color-critical-charge":
						if(++i < args.Length) battery.ColorCriticalCharge = ColorTranslator.FromHtml(args[i]);
						else throw new Exception("Critical charge color not supplied");
						break;
					case "--interval":
						if(++i < args.Length) interval = int.Parse(args[i]);
						else throw new Exception("Interval not supplied");
						break;
					case "--show-console":
						console = true;
						break;
					default:
						throw new Exception("Argument " + args[i] + " not recognized");
				}
			}
		} catch(Exception e) {
			Console.WriteLine(e.Message);
		}
	}
	
	public static void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e) {
		battery.UpdateBattery();
	}
	
	public static void OnPowerChanged(object source, PowerModeChangedEventArgs e) {
		battery.UpdateBattery();
	}
}
