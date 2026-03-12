# UROVO K388S Printer SDK for .NET MAUI

.NET MAUI/Xamarin wrapper for UROVO K388S printer SDK, providing scanning and printing functionality support.

## Platform Support

- Android

## Features

### Scanning Features
- **Multiple Trigger Methods**: Code trigger, hardware button trigger
- **Output Modes**: Broadcast mode, text box output mode
- **Barcode Types**: Support for various 1D/2D barcodes
- **Symbol Configuration**: Enable/disable specific barcode types
- **Trigger Lock**: Lock/unlock scan trigger key
- **Parameter Configuration**: Integer and string parameter configuration

### Printing Features
- **Basic Printing**: PrinterSDK API (text, barcode, QR code, images)
- **Advanced Printing**: PrinterConnector API (CPCL, ESC/POS raw commands)
- **Page Setup**: Width, height, rotation
- **Print Control**: Grayscale, speed, contrast adjustment
- **Paper Detection**: Label detection, black mark detection
- **Status Query**: Real-time printer status query

## Requirements

- **.NET**: 8.0+
- **MAUI/Xamarin**: Supports .NET MAUI and Xamarin.Android
- **Android SDK**: API 19+
- **Dependencies**: 
  - android.device.ScanManager
  - Com.Urovo.K388.PrinterSDK
  - Com.Urovo.K388.PrinterConnector

## Installation

### 1. Add DLL Reference

Add `K388_PrintSDK_Lib.dll` to project references.

### 2. ProGuard Configuration

If ProGuard is enabled, add the following rules:

```
-keep class android.content.** { *; }
-keep class android.os.* { *; }
-keep class android.device.* { *; }
-keep class com.urovo.* { *; }
```

## Scanning Features

### Quick Start

#### 1. Register Broadcast Receiver

```csharp
using Android.Content;
using Android.Device;

// Define broadcast receiver
private ScanBroadcastReceiver scanReceiver;

protected override void OnCreate(Bundle savedInstanceState)
{
    base.OnCreate(savedInstanceState);
    
    // Register broadcast
    scanReceiver = new ScanBroadcastReceiver(this);
    IntentFilter filter = new IntentFilter();
    filter.AddAction(ScanManager.ActionDecode);
    RegisterReceiver(scanReceiver, filter);
}

// Broadcast receiver class
class ScanBroadcastReceiver : BroadcastReceiver
{
    private MainActivity activity;
    
    public ScanBroadcastReceiver(MainActivity activity)
    {
        this.activity = activity;
    }
    
    public override void OnReceive(Context context, Intent intent)
    {
        if (intent.Action == ScanManager.ActionDecode)
        {
            string barcode = intent.GetStringExtra(ScanManager.BarcodeStringTag);
            int barcodeType = intent.GetIntExtra(ScanManager.BarcodeTypeTag, 0);
            
            activity.OnScanResult(barcode, barcodeType);
        }
    }
}
```

#### 2. Initialize Scanner

```csharp
using Android.Device;

private ScanManager scanManager;

private void InitScanner()
{
    scanManager = new ScanManager();
    
    // Open scanner
    bool isOpen = scanManager.OpenScanner();
    
    // Set output mode to broadcast mode
    // 0 - Broadcast mode, 1 - Text box output
    scanManager.SwitchOutputMode(0);
}
```

#### 3. Start/Stop Scanning

```csharp
// Start scanning
private void StartScan()
{
    if (scanManager != null)
    {
        bool started = scanManager.StartDecode();
        if (started)
        {
            Console.WriteLine("Scanning started successfully");
        }
    }
}

// Stop scanning
private void StopScan()
{
    if (scanManager != null)
    {
        bool stopped = scanManager.StopDecode();
        if (stopped)
        {
            Console.WriteLine("Scanning stopped");
        }
    }
}
```

#### 4. Clean Up Resources

```csharp
protected override void OnDestroy()
{
    base.OnDestroy();
    
    if (scanManager != null)
    {
        scanManager.StopDecode();
        scanManager.CloseScanner();
    }
    
    if (scanReceiver != null)
    {
        UnregisterReceiver(scanReceiver);
    }
}
```

### ScanManager API

#### Basic Control

##### OpenScanner()
Power on scanner

```csharp
bool isOpen = scanManager.OpenScanner();
```

**Returns:** `true`=success, `false`=failure

##### CloseScanner()
Power off scanner

```csharp
bool isClosed = scanManager.CloseScanner();
```

**Returns:** `true`=success, `false`=failure

##### GetScannerState()
Get scanner power state

```csharp
bool isOn = scanManager.GetScannerState();
```

**Returns:** `true`=powered on, `false`=powered off

#### Scan Control

##### StartDecode()
Start barcode decoding

```csharp
bool started = scanManager.StartDecode();
```

**Returns:** `true`=success, `false`=failure

**Note:** Scanner starts scanning, results sent via broadcast

##### StopDecode()
Stop barcode decoding

```csharp
bool stopped = scanManager.StopDecode();
```

**Returns:** `true`=success, `false`=failure

#### Output Mode

##### SwitchOutputMode(int mode)
Set scan result output mode

```csharp
bool switched = scanManager.SwitchOutputMode(0);
```

**Parameters:**
- `mode`: Output mode
  - `0`: Broadcast mode (recommended)
  - `1`: Text box output mode

**Returns:** `true`=success, `false`=failure

##### GetOutputMode()
Get current output mode

```csharp
int mode = scanManager.GetOutputMode();
```

**Returns:** `0`=broadcast mode, `1`=text box mode

#### Trigger Control

##### SetTriggerMode(Triggering mode)
Set trigger mode

```csharp
scanManager.SetTriggerMode(Triggering.Host);
```

**Parameters:**
- `Triggering.Host`: Host trigger (via code)
- `Triggering.Level`: Level trigger
- `Triggering.Pulse`: Pulse trigger

##### GetTriggerMode()
Get current trigger mode

```csharp
Triggering mode = scanManager.GetTriggerMode();
```

##### LockTrigger()
Lock scan trigger key (disable scan button)

```csharp
bool locked = scanManager.LockTrigger();
```

**Returns:** `true`=success, `false`=failure

##### UnlockTrigger()
Unlock scan trigger key (enable scan button)

```csharp
bool unlocked = scanManager.UnlockTrigger();
```

**Returns:** `true`=success, `false`=failure

##### GetTriggerLockState()
Get trigger key lock state

```csharp
bool isLocked = scanManager.GetTriggerLockState();
```

**Returns:** `true`=locked, `false`=unlocked

#### Symbol Type Management

##### EnableAllSymbologies(bool enable)
Enable/disable all supported barcode types

```csharp
scanManager.EnableAllSymbologies(true);  // Enable all
scanManager.EnableAllSymbologies(false); // Disable all
```

**Parameters:**
- `enable`: `true`=enable, `false`=disable

##### EnableSymbology(Symbology barcodeType, bool enable)
Enable/disable specific barcode type

```csharp
scanManager.EnableSymbology(Symbology.Code128, true);
scanManager.EnableSymbology(Symbology.QrCode, true);
```

**Parameters:**
- `barcodeType`: Barcode type (e.g., Code128, QrCode, etc.)
- `enable`: `true`=enable, `false`=disable

##### IsSymbologyEnabled(Symbology barcodeType)
Check if specific barcode type is enabled

```csharp
bool isEnabled = scanManager.IsSymbologyEnabled(Symbology.Code128);
```

**Returns:** `true`=enabled, `false`=disabled

##### IsSymbologySupported(Symbology barcodeType)
Check if device supports specific barcode type

```csharp
bool isSupported = scanManager.IsSymbologySupported(Symbology.DataMatrix);
```

**Returns:** `true`=supported, `false`=not supported

#### Parameter Configuration

##### SetParameterInts(int[] idBuffer, int[] valueBuffer)
Set integer type parameters

```csharp
int[] ids = { paramId1, paramId2 };
int[] values = { value1, value2 };
int result = scanManager.SetParameterInts(ids, values);
```

**Returns:** `0`=success, other values=failure

##### GetParameterInts(int[] idBuffer)
Get integer type parameters

```csharp
int[] ids = { paramId1, paramId2 };
int[] values = scanManager.GetParameterInts(ids);
```

**Returns:** Parameter value array

##### SetParameterString(int[] idBuffer, string[] valueBuffer)
Set string type parameters

```csharp
int[] ids = { paramId1 };
string[] values = { "value1" };
bool result = scanManager.SetParameterString(ids, values);
```

**Returns:** `true`=success, `false`=failure

##### GetParameterString(int[] idBuffer)
Get string type parameters

```csharp
int[] ids = { paramId1 };
string[] values = scanManager.GetParameterString(ids);
```

**Returns:** Parameter value array

##### ResetScannerParameters()
Reset all parameters to factory defaults

```csharp
bool reset = scanManager.ResetScannerParameters();
```

**Returns:** `true`=success, `false`=failure

## Printing Features

### Method 1: PrinterSDK (Recommended for Basic Printing)

#### Quick Start

```csharp
using Com.Urovo.K388;

// 1. Get PrinterSDK instance
PrinterSDK printer = PrinterSDK.GetInstance();

// 2. Set page size (8 pixels = 1mm)
int maxWidth = 50 * 8;   // 50mm wide
int maxHeight = 30 * 8;  // 30mm high
printer.PageSetup(maxWidth, maxHeight);

// 3. Add print content
int currentY = 10;

// Print text
printer.DrawText(0, currentY, "Label Print Test", 24, 0, 0, false, false);
currentY += 40;

// Print barcode
printer.DrawBarCode(0, currentY, "20230815143001", 1, 0, 3, 100);
currentY += 120;

// Print QR code
printer.DrawQrCode(100, currentY, "https://www.urovo.com", 0, 4, 2);

// 4. Execute print
printer.Print(0, new PrintListenerImpl());

// Print listener
class PrintListenerImpl : Java.Lang.Object, IPrintListener
{
    public void OnPrintResult(int result)
    {
        if (result == 0)
        {
            Console.WriteLine("Print successful");
        }
        else
        {
            Console.WriteLine($"Print failed: {result}");
        }
    }
}
```

#### PrinterSDK API

##### GetInstance()
Get PrinterSDK singleton instance

```csharp
PrinterSDK printer = PrinterSDK.GetInstance();
```

##### PageSetup(int pageWidth, int pageHeight)
Set page size

```csharp
printer.PageSetup(400, 240);  // Width 400 pixels, height 240 pixels
```

**Parameters:**
- `pageWidth`: Page width (pixels), 8 pixels = 1mm
- `pageHeight`: Page height (pixels)

##### PageSetup(int pageWidth, int pageHeight, int rotation)
Set page size (with rotation)

```csharp
printer.PageSetup(400, 240, 90);  // Rotate 90 degrees
```

**Parameters:**
- `rotation`: Rotation angle (0, 90, 180, 270)

#### Text Printing

##### DrawText(int x, int y, string text, int fontSize, int rotate, int bold, bool reverse, bool underline)
Print text (simplified version)

```csharp
printer.DrawText(0, 10, "Hello World", 24, 0, 0, false, false);
```

**Parameters:**
- `x, y`: Starting coordinates (pixels)
- `text`: Text content
- `fontSize`: Font size
- `rotate`: Rotation angle (0/90/180/270)
- `bold`: Bold level (0=no bold, >1=bold)
- `reverse`: Reverse print
- `underline`: Underline

##### DrawText(int x, int y, string text, int fontType, int fontSize, int rotate, int bold, bool reverse, bool underline)
Print text (full version)

```csharp
printer.DrawText(0, 10, "Hello World", 24, 32, 0, 1, false, false);
```

**Parameters:**
- `fontType`: Font type
- Other parameters same as simplified version

##### DrawText(int x, int y, string text, string fontType, int fontSize, int rotate, int bold, bool reverse, bool underline)
Print text (font name)

```csharp
printer.DrawText(0, 10, "Hello World", "Arial", 24, 0, 0, false, false);
```

**Parameters:**
- `fontType`: Font name (e.g., "Arial", "Times", etc.)

##### MultLine(int height, int fontType, int fontSize, int x, int y, int rotate, params string[] strs)
Multi-line print

```csharp
printer.MultLine(200, 24, 24, 0, 10, 0, "Line 1", "Line 2", "Line 3");
```

**Parameters:**
- `height`: Total height of multi-line area
- `strs`: Multi-line text array

#### Barcode Printing

##### DrawBarCode(int x, int y, string text, int type, int rotate, int linewidth, int height)
Print barcode

```csharp
printer.DrawBarCode(0, 50, "1234567890", 1, 0, 3, 100);
```

**Parameters:**
- `x, y`: Starting coordinates
- `text`: Barcode content
- `type`: Barcode type
  - `1`: Code128
  - `2`: Code39
  - `3`: EAN13
  - `4`: EAN8
  - See SDK documentation for other values
- `rotate`: Rotation angle
- `linewidth`: Line width (1-4)
- `height`: Barcode height (pixels)

##### BarcodeText(int font, int size, int offset, int rotate, int width, int ratio, int height, int x, int y, string data)
Barcode identifier (with text)

```csharp
printer.BarcodeText(24, 16, 0, 0, 3, 2, 100, 0, 50, "1234567890");
```

#### QR Code Printing

##### DrawQrCode(int x, int y, string text, int rotate, int ver, int lel)
Print QR code

```csharp
printer.DrawQrCode(100, 100, "https://www.urovo.com", 0, 4, 2);
```

**Parameters:**
- `x, y`: Starting coordinates
- `text`: QR code content
- `rotate`: Rotation angle
- `ver`: Version (1-10, higher number = larger QR code)
- `lel`: Error correction level
  - `1`: L level (7% correction)
  - `2`: M level (15% correction)
  - `3`: Q level (25% correction)
  - `4`: H level (30% correction)

#### Image Printing

##### DrawGraphic(int x, int y, Bitmap bmp)
Print image

```csharp
Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.logo);
printer.DrawGraphic(0, 100, bitmap);
```

**Parameters:**
- `x, y`: Starting coordinates
- `bmp`: Bitmap object

#### Graphics Drawing

##### DrawLine(int lineWidth, int x0, int y0, int x1, int y1)
Print straight line

```csharp
printer.DrawLine(2, 0, 100, 400, 100);  // Horizontal line
```

**Parameters:**
- `lineWidth`: Line width (pixels)
- `x0, y0`: Start coordinates
- `x1, y1`: End coordinates

##### DrawBox(int lineWidth, int x0, int y0, int x1, int y1)
Print rectangle box

```csharp
printer.DrawBox(2, 10, 10, 390, 230);
```

**Parameters:**
- `lineWidth`: Border line width
- `x0, y0`: Top-left corner coordinates
- `x1, y1`: Bottom-right corner coordinates

##### DrawINVERSE(int x0, int y0, int x1, int y1, int width)
Print inverse line segment

```csharp
printer.DrawINVERSE(0, 50, 400, 70, 20);
```

**Parameters:**
- `width`: Line segment width

#### Print Control

##### Print(int skip, IPrintListener listener)
Execute print

```csharp
printer.Print(0, new PrintListenerImpl());
```

**Parameters:**
- `skip`: Paper detection method
  - `0`: No feed (direct print)
  - `1`: Label detection
  - `2`: Left black mark detection
  - `3`: Right black mark detection
- `listener`: Print result listener

##### Print(IPrintListener listener)
Execute print (default no feed)

```csharp
printer.Print(new PrintListenerImpl());
```

##### Speed(int level)
Set print speed

```csharp
printer.Speed(3);
```

**Parameters:**
- `level`: Speed level (0-5)
  - `0`: Slowest
  - `5`: Fastest

##### ContRast(int level)
Set contrast

```csharp
printer.ContRast(2);
```

**Parameters:**
- `level`: Contrast level
  - `0`: Default
  - `1`: Medium
  - `2`: Dark
  - `3`: Very dark
  
**Note:** Higher contrast = darker print, but slower speed

##### BackGround(int level)
Set watermark grayscale level

```csharp
printer.BackGround(1);
```

**Parameters:**
- `level`: Grayscale level

##### SetBold(int level)
Set bold font

```csharp
printer.SetBold(1);  // Enable bold
printer.SetBold(0);  // Cancel bold
```

**Parameters:**
- `level`: `0`=cancel bold, `>1`=enable bold

#### Advanced Features

##### Prefeed(int len)
Pre-feed before printing

```csharp
printer.Prefeed(20);  // Feed 20 pixels before printing
```

##### Postfeed(int len)
Post-feed after printing

```csharp
printer.Postfeed(30);  // Feed 30 pixels after printing
```

##### SetPrintTime(int time)
Set number of print copies

```csharp
printer.SetPrintTime(3);  // Print 3 copies
```

##### Count(int num)
Auto-increment/decrement number

```csharp
printer.Count(1);   // Increment
printer.Count(-1);  // Decrement
```

**Parameters:**
- `num`: Increment/decrement value (±65535)

##### PrintWait(int time)
Delayed print

```csharp
printer.PrintWait(8);  // Delay 1 second (8 * 1/8 second)
```

**Parameters:**
- `time`: Delay time (1/8 second units)

##### SetPace()
Batch printing (press feed button to print next)

```csharp
printer.SetPace();
```

##### AlignLeft()
Left align

```csharp
printer.AlignLeft();
```

##### AlignCenter()
Center align

```csharp
printer.AlignCenter();
```

##### AlignRight()
Right align

```csharp
printer.AlignRight();
```

##### UnderLine(bool mode)
Underline

```csharp
printer.UnderLine(true);   // Enable
printer.UnderLine(false);  // Disable
```

##### SetMag(int w, int h)
Character magnification

```csharp
printer.SetMag(2, 2);  // Magnify both width and height by 2x
```

##### SetSP(int spacing)
Character spacing

```csharp
printer.SetSP(4);  // Spacing 4 * 0.125mm
```

##### GetPrintBytes()
Get current print data

```csharp
byte[] printData = printer.GetPrintBytes();
```

**Returns:** Print data byte array

##### Version()
Get SDK version information

```csharp
string version = printer.Version();
```

### Method 2: PrinterConnector (Advanced Printing)

Used for sending CPCL, ESC/POS raw print commands.

#### Quick Start

```csharp
using Com.Urovo.K388;
using System.Text;

// Must execute in background thread
await Task.Run(() =>
{
    PrinterConnector connector = new PrinterConnector();
    
    try
    {
        // 1. Connect printer
        bool isOpen = connector.Open();
        if (!isOpen)
        {
            Console.WriteLine("Failed to connect printer");
            return;
        }
        
        // 2. Clear buffer
        connector.Flush();
        
        // 3. Check printer status
        PrinterConnector.PrinterResult status = connector.Status();
        if (status != PrinterConnector.PrinterResult.Ok)
        {
            Console.WriteLine($"Printer status abnormal: {status}");
            connector.Close();
            return;
        }
        
        // 4. Build CPCL commands
        StringBuilder cpcl = new StringBuilder();
        cpcl.Append("! 0 200 200 240 1\r\n");
        cpcl.Append("CENTER\r\n");
        cpcl.Append("PAGE-WIDTH 400\r\n");
        cpcl.Append("T 24 1 0 0 Label CPCL Print Test\r\n");
        cpcl.Append("T 24 1 0 35 Barcode: 20230815143001\r\n");
        cpcl.Append("BARCODE 128 2 1 100 0 70 20230815143001\r\n");
        cpcl.Append("GAP-SENSE\r\n");
        cpcl.Append("FORM\r\n");
        cpcl.Append("PRINT\r\n");
        
        // 5. Convert to GBK encoding (supports Chinese)
        byte[] bytes = Encoding.GetEncoding("GBK").GetBytes(cpcl.ToString());
        
        // 6. GZIP compression (optional)
        byte[] gzipBytes = GZIPFrame.Codec(bytes);
        
        // 7. Send data
        bool written = connector.Write(gzipBytes, gzipBytes.Length);
        if (written)
        {
            Console.WriteLine("CPCL command sent successfully");
        }
        
        // 8. Close connection
        connector.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Print failed: {ex.Message}");
        connector.Close();
    }
});
```

#### PrinterConnector API

##### Open()
Connect printer

```csharp
bool isOpen = connector.Open();
```

**Returns:** `true`=connection successful, `false`=connection failed

**Note:** Must be called in background thread

##### Close()
Disconnect printer

```csharp
connector.Close();
```

##### IsConnected()
Get printer connection status

```csharp
bool isConnected = connector.IsConnected();
```

**Returns:** `true`=connected, `false`=not connected

##### Flush()
Clear print response buffer

```csharp
connector.Flush();
```

**Note:** Recommended to call this method first after successful connection

##### Status()
Get printer status

```csharp
PrinterConnector.PrinterResult status = connector.Status();
```

**Returns:**
- `PrinterResult.Ok`: Normal
- `PrinterResult.OutOfPaper`: Out of paper
- `PrinterResult.OverHeat`: Overheating
- See SDK for other statuses

##### Write(byte[] data, int len)
Send print data

```csharp
bool written = connector.Write(data, data.Length);
```

**Parameters:**
- `data`: Print data byte array
- `len`: Data length

**Returns:** `true`=send successful, `false`=send failed

##### Write(string str)
Send print data (string)

```csharp
bool written = connector.Write(cpclString);
```

**Parameters:**
- `str`: Print command string

**Returns:** `true`=send successful, `false`=send failed

##### Read(byte[] data, int readLen, int timeout_ms)
Read printer response

```csharp
byte[] buffer = new byte[1024];
int bytesRead = connector.Read(buffer, 1024, 5000);
```

**Parameters:**
- `data`: Receive buffer
- `readLen`: Read length
- `timeout_ms`: Timeout (milliseconds)

**Returns:** Actual bytes read

## CPCL Print Example

### Basic CPCL Template

```csharp
// CPCL label print template
string cpcl = "! 0 200 200 240 1\r\n"           // Initialize: offset 0, height 200, quantity 1
            + "CENTER\r\n"                       // Center align
            + "PAGE-WIDTH 400\r\n"               // Page width 400 dots
            + "T 24 1 0 0 Title Text\r\n"        // Text: font 24, rotation 1, coords (0,0)
            + "T 16 1 0 35 Content Line 1\r\n"   // Text: font 16
            + "BARCODE 128 2 1 100 0 70 1234567890\r\n"  // Barcode: Code128
            + "FORM\r\n"                         // End form
            + "PRINT\r\n";                       // Print
```

### Complete Print Flow

```csharp
private async Task PrintCPCL(string cpclCommand)
{
    await Task.Run(() =>
    {
        PrinterConnector connector = new PrinterConnector();
        
        try
        {
            // Connect
            if (!connector.Open())
            {
                ShowMessage("Connection failed");
                return;
            }
            
            // Clear buffer
            connector.Flush();
            
            // Check status
            var status = connector.Status();
            if (status != PrinterConnector.PrinterResult.Ok)
            {
                ShowMessage($"Printer status: {status}");
                connector.Close();
                return;
            }
            
            // Encoding conversion (supports Chinese)
            byte[] bytes = Encoding.GetEncoding("GBK").GetBytes(cpclCommand);
            
            // Optional: GZIP compression
            byte[] compressedBytes = GZIPFrame.Codec(bytes);
            
            // Send
            bool written = connector.Write(compressedBytes, compressedBytes.Length);
            
            if (written)
            {
                ShowMessage("Print successful");
            }
            else
            {
                ShowMessage("Send failed");
            }
            
            // Close
            connector.Close();
        }
        catch (Exception ex)
        {
            ShowMessage($"Error: {ex.Message}");
            connector.Close();
        }
    });
}
```

## Complete Print Example

```csharp
using Com.Urovo.K388;
using Android.Graphics;

public class PrintManager
{
    private PrinterSDK printer;
    
    public PrintManager()
    {
        printer = PrinterSDK.GetInstance();
    }
    
    // Print label example
    public void PrintLabel(string title, string barcode, Bitmap logo)
    {
        // 1. Set page (50mm x 30mm)
        int width = 50 * 8;
        int height = 30 * 8;
        printer.PageSetup(width, height);
        
        int currentY = 10;
        
        // 2. Print title (centered)
        printer.AlignCenter();
        printer.SetBold(1);
        printer.DrawText(0, currentY, title, 32, 0, 0, false, false);
        printer.SetBold(0);
        printer.AlignLeft();
        currentY += 50;
        
        // 3. Print separator line
        printer.DrawLine(2, 0, currentY, width, currentY);
        currentY += 10;
        
        // 4. Print logo (if available)
        if (logo != null)
        {
            printer.DrawGraphic(10, currentY, logo);
            currentY += logo.Height + 10;
        }
        
        // 5. Print barcode
        printer.DrawBarCode(50, currentY, barcode, 1, 0, 3, 80);
        currentY += 100;
        
        // 6. Print barcode text
        printer.DrawText(70, currentY, barcode, 20, 0, 0, false, false);
        currentY += 30;
        
        // 7. Print QR code
        printer.DrawQrCode(width - 120, currentY - 120, 
            $"https://example.com?code={barcode}", 0, 4, 2);
        
        // 8. Set print parameters
        printer.Speed(3);        // Medium speed
        printer.ContRast(1);     // Medium contrast
        
        // 9. Execute print
        printer.Print(1, new PrintListener());  // 1=label detection
    }
    
    // Print listener
    class PrintListener : Java.Lang.Object, IPrintListener
    {
        public void OnPrintResult(int result)
        {
            switch (result)
            {
                case 0:
                    Console.WriteLine("Print successful");
                    break;
                case -1:
                    Console.WriteLine("Print failed: Out of paper");
                    break;
                case -2:
                    Console.WriteLine("Print failed: Overheating");
                    break;
                case -3:
                    Console.WriteLine("Print failed: Low voltage");
                    break;
                default:
                    Console.WriteLine($"Print failed: Error code{result}");
                    break;
            }
        }
    }
}
```

## Important Notes

### Scanning Features

1. **Broadcast Mode Recommended**: Setting output mode to 0 (broadcast mode) is more stable
2. **Resource Release**: Must unregister broadcast receiver when Activity is destroyed
3. **Hardware Button**: Device physical scan key automatically triggers scanning
4. **Trigger Lock**: Use `LockTrigger()` to disable hardware scan key

### Printing Features

1. **Coordinate System**: Origin (0,0) is at top-left, 8 pixels = 1mm
2. **Page Setup**: Must call `PageSetup()` before adding print content
3. **Y Coordinate Accumulation**: Manually maintain `currentY` to avoid content overlap
4. **Print Order**: 
   ```
   PageSetup() → Draw...() → Set parameters → Print()
   ```

5. **PrinterConnector Notes**:
   - Must be used in background thread
   - Call `Flush()` first after connection to clear buffer
   - Check `Status()` to ensure printer is normal
   - Must call `Close()` after use

6. **Chinese Support**: 
   - PrinterSDK automatically supports Chinese
   - PrinterConnector needs to use GBK encoding

7. **CPCL Commands**: 
   - Use `\r\n` as line separator
   - Coordinate unit is dots
   - Optional GZIP compression for improved transmission efficiency

## Troubleshooting

### Scanning Issues

**Scanner Not Responding**
- Check if `OpenScanner()` returns true
- Confirm output mode setting is correct
- Verify broadcast receiver is registered

**Not Receiving Scan Results**
- Check if broadcast Action is `ScanManager.ActionDecode`
- Confirm output mode is 0 (broadcast mode)
- View logcat logs

### Printing Issues

**No Print Content**
- Confirm `PageSetup()` was called
- Check if Y coordinates are within page range
- Verify `Print()` was called

**PrinterConnector Connection Failed**
- Confirm called in background thread
- Check if printer is occupied by other app
- View printer status

**Chinese Garbled Characters**
- Use GBK encoding for PrinterConnector
- Check string encoding conversion

**Print Content Overlapping**
- Correctly accumulate Y coordinates
- Use returned actual height to update currentY

## Version Requirements

- **.NET**: 8.0+ or Xamarin
- **Android SDK**: API 19+
- **Dependencies**: 
  - K388_PrintSDK_Lib.dll
  - android.device.ScanManager
  - Com.Urovo.K388.*

## Technical Support

For technical support, please contact UROVO technical support team.

## License

Copyright © UROVO Technology Co., Ltd.
