using System;
using System.Linq;
using System.Drawing;
using System.Management;
using System.Windows.Forms;

public class Battery  {
	public static readonly int[,] BATTERY_ICON = {
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
		{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
		{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
		{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
		{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
		{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
		{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
		{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
		{ 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
		{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
	};
	
	public static readonly int CHARGE_START_X 	= 2;
	public static readonly int CHARGE_START_Y 	= 5;
	public static readonly int CHARGE_END_X 	= 12;
	public static readonly int CHARGE_END_Y 	= 10;
	
	public Color ColorOutline			{ get; set; }
	public Color ColorBackground		{ get; set; }
	public Color ColorNormalCharge		{ get; set; }
	public Color ColorPowerSaveCharge	{ get; set; }
	public Color ColorLowCharge			{ get; set; }
	public Color ColorCriticalCharge	{ get; set; }

	public Color[] BatteryColors { get; private set; }
	
	public static ManagementObjectSearcher mos { get; private set; }
	
	public NotifyIcon notifyIcon { get; private set; }
	
	static Battery() {
		mos = new ManagementObjectSearcher("select * from Win32_Battery");
	}
	
	public Battery() {
		this.ColorOutline			= Color.White;
		this.ColorBackground		= Color.Transparent;
		this.ColorNormalCharge		= Color.White;
		this.ColorPowerSaveCharge 	= Color.Green;
		this.ColorLowCharge			= Color.Yellow;
		this.ColorCriticalCharge	= Color.Red;
		
		this.BatteryColors = new Color[] { this.ColorBackground, this.ColorOutline };
	
		this.notifyIcon = new NotifyIcon();
		this.notifyIcon.Visible = true;
		ContextMenu menu = new ContextMenu();
		MenuItem exit = new MenuItem();
		exit.Text = "E&xit";
		exit.Click += HandleExit;
		menu.MenuItems.AddRange(new MenuItem[] {
			exit
		});
		this.notifyIcon.ContextMenu = menu;
	}
	
	private void HandleExit(object sender, EventArgs e) { 
		this.notifyIcon.Icon = null;
		this.notifyIcon.Visible = false;
		Environment.Exit(0); 
	}
	
	private Bitmap DrawBattery(uint charge, Color chargeColor) {
		Bitmap bmp = new Bitmap(BATTERY_ICON.GetLength(1), BATTERY_ICON.GetLength(0));
		
		// Draw the whole battery
		for(int y = 0; y < BATTERY_ICON.GetLength(0); y++) {
			for(int x = 0; x < BATTERY_ICON.GetLength(1); x++) {
				Color color = this.ColorBackground;
				switch(BATTERY_ICON[y, x]) {
					case 0: color = this.ColorBackground; break;
					case 1: color = this.ColorOutline; break;
					
				}
				bmp.SetPixel(x, y, color);
			}
		}
		
		// Draw the charge percentage
		int partialChargeX = (int) (CHARGE_START_X + (charge / 10));
		for(int y = CHARGE_START_Y; y <= CHARGE_END_Y; y++) {
			for(int x = CHARGE_START_X; x <= partialChargeX; x++) {
				bmp.SetPixel(x, y, chargeColor);
			}
		}
		
		return bmp;
	}
	
	public void UpdateBattery() {
		ManagementObject battery = GetWmiBattery();
		uint charge = Convert.ToUInt32(battery["EstimatedChargeRemaining"]);
		uint status = Convert.ToUInt32(battery["BatteryStatus"]);
		
		Bitmap bmp = DrawBattery(charge, GetChargeColor(status));
		this.notifyIcon.Icon = Icon.FromHandle(bmp.GetHicon());
		this.notifyIcon.Text = (new int[] { 2, 6, 7, 8, 9 }.Contains((int) status) ?
								"[Charging]" : "[Discharging]")
							+ "\nCharge: " + charge + "%";
	}
	
	public Color GetChargeColor(uint status) {
		switch(status) {
			case 4: return ColorLowCharge;		// Low
			case 5: return ColorCriticalCharge;	// Critical
			case 6: return ColorNormalCharge;	// Charging
			case 7: return ColorNormalCharge;	// Charging and high
			case 8: return ColorLowCharge;		// Charging and low
			case 9: return ColorCriticalCharge;	// Charging and critical
		}
		
		return ColorNormalCharge;
	}
	
	public static ManagementObject GetWmiBattery() {
		var enumerator = mos.Get().GetEnumerator();
		if(!enumerator.MoveNext()) throw new Exception("No battery found");
		return (ManagementObject) enumerator.Current;
	}
}
