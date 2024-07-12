using Microsoft.OpenApi.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Gpio;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;

public class RaspberryPiController
{
    /// PINOUT
    ///                   3.3V - 01 | 02 - 5V
    ///   [I2C1 (SDA)] GPIO 02 - 03 | 04 - 5V
    ///   [I2C1 (SCL)] GPIO 03 - 05 | 06 - GND
    ///       (GPCLK0) GPIO 04 - 07 | 08 - GPIO 14 [(TxD) UART]
    ///                    GND - 09 | 10 - GPIO 15 [(RxD) UART]
    ///                GPIO 17 - 11 | 12 - GPIO 18 (PWM0)
    ///                GPIO 27 - 13 | 14 - GND
    ///                GPIO 22 - 15 | 16 - GPIO 23
    ///                   3.3V - 17 | 18 - GPIO 24
    ///  [SPI0 (MOSI)] GPIO 10 - 19 | 20 - GND
    ///  [SPI0 (MISO)] GPIO 09 - 21 | 22 - GPIO 25
    ///  [SPI0 (SCLK)] GPIO 11 - 23 | 24 - GPIO 08 [(CE0) SPI0]
    ///                    GND - 25 | 26 - GPIO 07 [(CE1) SPI0]
    /// [I2C0 (ID_SD)] GPIO 00 - 27 | 28 - GPIO 01 [(ID_SC) I2C0]
    ///       (GPCLK1) GPIO 05 - 29 | 30 - GND
    ///       (GPCLK2) GPIO 06 - 31 | 32 - GPIO 12 (PWM0)
    ///         (PWM1) GPIO 13 - 33 | 34 - GND
    ///  [SPI1 (MISO)] GPIO 19 - 35 | 36 - GPIO 16
    ///                GPIO 26 - 37 | 38 - GPIO 20 [(MOSI) SPI1]
    ///                    GND - 39 | 40 - GPIO 21 [(SCLK) SPI1]

    private readonly GpioController _controller;


    public enum Pinout { vout_3_3V, vout_5v, GPIO, GND };

    /// <summary>
    /// Default state of the GPIO
    /// </summary>
    private readonly PinMode[] _pinModes = [
        PinMode.InputPullUp,
        PinMode.InputPullUp,
        PinMode.InputPullUp,
        PinMode.InputPullUp,
        PinMode.InputPullUp,
        PinMode.InputPullUp,
        PinMode.InputPullUp,
        PinMode.InputPullUp,
        PinMode.InputPullUp,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown,
        PinMode.InputPullDown
    ];

    private static Lazy<IReadOnlyDictionary<int, int>> _gpioToPin_Lazy = new Lazy<IReadOnlyDictionary<int, int>>(() => new Dictionary<int, int>{
        {  0, 27 }, {  1, 28 }, {  2,  3 }, {  3,  5 }, {  4,  7 }, {  5, 29 }, {  6, 31 }, {  7, 26 }, {  8, 24 }, {  9, 21 },
        { 10, 19 }, { 11, 23 }, { 12, 32 }, { 13, 33 }, { 14,  8 }, { 15, 10 }, { 16, 36 }, { 17, 11 }, { 18, 12 }, { 19, 35 },
        { 20, 38 }, { 21, 40 }, { 22, 15 }, { 23, 16 }, { 24, 18 }, { 25, 22 }, { 26, 37 }, { 27, 13 }
    });
    private static Lazy<IReadOnlyDictionary<int, int>> _pinToGpio_Lazy = new Lazy<IReadOnlyDictionary<int, int>>(() => _gpioToPin_Lazy.Value.ToDictionary(x => x.Value, x => x.Key));
    //private static Lazy<IReadOnlyDictionary<int, int>> _pinToGpio_Lazy = new Lazy<IReadOnlyDictionary<int, int>>(() => new Dictionary<int, int>{
    //    {  3,  2 }, {  5,  3 }, {  7,  4 }, {  8, 14 }, { 10, 15 }, { 11, 17 }, { 12, 18 }, { 13, 27 }, { 15, 22 }, { 16, 23 },
    //    { 18, 24 }, { 19, 10 }, { 21,  9 }, { 22, 25 }, { 23, 11 }, { 24,  8 }, { 26,  7 }, { 27,  0 }, { 28,  1 }, { 29,  5 },
    //    { 31,  6 }, { 32, 12 }, { 33, 13 }, { 35, 19 }, { 36, 16 }, { 37, 26 }, { 38, 20 }, { 40, 21 }
    //});
    private static Lazy<IReadOnlyCollection<Pinout>> _pinouts = new Lazy<IReadOnlyCollection<Pinout>>(() => new List<Pinout> {
        Pinout.vout_3_3V,   Pinout.vout_5v,     // 01 | 02
        Pinout.GPIO,        Pinout.vout_5v,     // 03 | 04
        Pinout.GPIO,        Pinout.GND,         // 05 | 06
        Pinout.GPIO,        Pinout.GPIO,        // 07 | 08
        Pinout.GND,         Pinout.GPIO,        // 09 | 10
        Pinout.GPIO,        Pinout.GPIO,        // 11 | 12
        Pinout.GPIO,        Pinout.GND,         // 13 | 14
        Pinout.GPIO,        Pinout.GPIO,        // 15 | 16        
        Pinout.vout_3_3V,   Pinout.GPIO,        // 17 | 18
        Pinout.GPIO,        Pinout.GND,         // 19 | 20
        Pinout.GPIO,        Pinout.GPIO,        // 21 | 22
        Pinout.GPIO,        Pinout.GPIO,        // 23 | 24
        Pinout.GND,         Pinout.GPIO,        // 25 | 26
        Pinout.GPIO,        Pinout.GPIO,        // 27 | 28
        Pinout.GPIO,        Pinout.GND,         // 29 | 30
        Pinout.GPIO,        Pinout.GPIO,        // 31 | 32
        Pinout.GPIO,        Pinout.GND,         // 33 | 34
        Pinout.GPIO,        Pinout.GPIO,        // 35 | 36
        Pinout.GPIO,        Pinout.GPIO,        // 37 | 38
        Pinout.GND,         Pinout.GPIO,        // 39 | 40
    }.AsReadOnly());
    public static IReadOnlyDictionary<int, int> GpioToPin => _gpioToPin_Lazy.Value;
    public static IReadOnlyDictionary<int, int> PinToGpio => _pinToGpio_Lazy.Value;
    public static IReadOnlyCollection<Pinout> Pinouts => _pinouts.Value;


    public enum ProtocolPinout {
        SDA, SCL,
        GPCLK0, GPCLK1, GPCLK2,
        GPCLK, MOSI, MISO, SCLK,
        CE0, CE1,
        ID_SD, ID_SC,
        TxD, RxD,
        PWM0, PWM1 };
    public enum ProtocolOut { I2C0, I2C1, SPI0, SPI1, UART, PWM };

    private readonly ProtocolPinout[] _protocolPinout = [

    ];
    private readonly ProtocolOut[] _porotocolOuts = [
    ];

    private record ProtocolInfo(
        ProtocolOut Protocol,
        bool Enabled,
        IDictionary<ProtocolPinout, int> ProtocolPinInfo);
    private readonly Dictionary<ProtocolOut, ProtocolInfo> _porotocolInfos = new Dictionary<ProtocolOut, ProtocolInfo> {
        { ProtocolOut.I2C0, 
            new(ProtocolOut.I2C0, 
                false, 
                new Dictionary<ProtocolPinout, int>(){ 
                    { ProtocolPinout.SDA, 1 }, 
                    { ProtocolPinout.SCL, 1 } 
                })
        }
    };


    public RaspberryPiController(IServiceProvider serviceProvider)
    {
        _controller = new GpioController();
    }

    public void ReadAll()
    {
        
    }

    public void SetMode()
    {

    }
}