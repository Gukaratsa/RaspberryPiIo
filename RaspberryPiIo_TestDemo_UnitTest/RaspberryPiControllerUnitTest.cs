namespace RaspberryPiIo_TestDemo_UnitTest
{
    public class RaspberryPiControllerUnitTest
    {
        [Fact]
        public void GpioToPin_Matches_PinToGpio()
        {
            foreach(var v_orig in RaspberryPiController.PinToGpio)
            {
                var v_targ = RaspberryPiController.GpioToPin[v_orig.Value];
                Assert.Equal(v_orig.Key, v_targ);
            }
        }

        [Fact]
        public void PinToGpio_Matches_GpioToPin()
        {
            foreach (var v_orig in RaspberryPiController.GpioToPin)
            {
                var v_targ = RaspberryPiController.PinToGpio[v_orig.Value];
                Assert.Equal(v_orig.Key, v_targ);
            }
        }

        [Fact]
        public void GpioToPin_Matches_Pinout()
        {
            foreach (var v_orig in RaspberryPiController.GpioToPin)
            {
                var v_targ = RaspberryPiController.DefaultPinouts.ElementAt(v_orig.Value - 1);
                Assert.Equal(RaspberryPiController.Pinout.GPIO, v_targ);
            }
        }

        [Fact]
        public void GetPinouts_ShouldReturnDeafult_WhenNoProtocolEnabled()
        {
            RaspberryPiController sut = new();
            var pinouts = sut.GetPinouts();
            var expected = """
                                vout_3_3V - 01 | 02 - vout_5v                  
                              InputPullUp - 03 | 04 - vout_5v                  
                              InputPullUp - 05 | 06 - GND                      
                              InputPullUp - 07 | 08 - InputPullDown            
                                      GND - 09 | 10 - InputPullDown            
                            InputPullDown - 11 | 12 - InputPullDown            
                            InputPullDown - 13 | 14 - GND                      
                            InputPullDown - 15 | 16 - InputPullDown            
                                vout_3_3V - 17 | 18 - InputPullDown            
                            InputPullDown - 19 | 20 - GND                      
                            InputPullDown - 21 | 22 - InputPullDown            
                            InputPullDown - 23 | 24 - InputPullUp              
                                      GND - 25 | 26 - InputPullUp              
                              InputPullUp - 27 | 28 - InputPullUp              
                              InputPullUp - 29 | 30 - GND                      
                              InputPullUp - 31 | 32 - InputPullDown            
                            InputPullDown - 33 | 34 - GND                      
                            InputPullDown - 35 | 36 - InputPullDown            
                            InputPullDown - 37 | 38 - InputPullDown            
                                      GND - 39 | 40 - InputPullDown            
                """;
            Assert.Equal(expected, pinouts);
        }

        [Fact]
        public void GetPinouts_ShouldReturnChanged_WhenModeChanged_00_Output()
        {
            RaspberryPiController sut = new();
            sut.SetMode(0, System.Device.Gpio.PinMode.Output);
            var pinouts = sut.GetPinouts();
            var expected = """
                                vout_3_3V - 01 | 02 - vout_5v                  
                              InputPullUp - 03 | 04 - vout_5v                  
                              InputPullUp - 05 | 06 - GND                      
                              InputPullUp - 07 | 08 - InputPullDown            
                                      GND - 09 | 10 - InputPullDown            
                            InputPullDown - 11 | 12 - InputPullDown            
                            InputPullDown - 13 | 14 - GND                      
                            InputPullDown - 15 | 16 - InputPullDown            
                                vout_3_3V - 17 | 18 - InputPullDown            
                            InputPullDown - 19 | 20 - GND                      
                            InputPullDown - 21 | 22 - InputPullDown            
                            InputPullDown - 23 | 24 - InputPullUp              
                                      GND - 25 | 26 - InputPullUp              
                                   Output - 27 | 28 - InputPullUp              
                              InputPullUp - 29 | 30 - GND                      
                              InputPullUp - 31 | 32 - InputPullDown            
                            InputPullDown - 33 | 34 - GND                      
                            InputPullDown - 35 | 36 - InputPullDown            
                            InputPullDown - 37 | 38 - InputPullDown            
                                      GND - 39 | 40 - InputPullDown            
                """;
            Assert.Equal(expected, pinouts);
        }

        [Fact]
        public void GetPinouts_ShouldReturnChanged_WhenModeChanged_00_Input()
        {
            RaspberryPiController sut = new();
            sut.SetMode(0, System.Device.Gpio.PinMode.Input);
            var pinouts = sut.GetPinouts();
            var expected = """
                                vout_3_3V - 01 | 02 - vout_5v                  
                              InputPullUp - 03 | 04 - vout_5v                  
                              InputPullUp - 05 | 06 - GND                      
                              InputPullUp - 07 | 08 - InputPullDown            
                                      GND - 09 | 10 - InputPullDown            
                            InputPullDown - 11 | 12 - InputPullDown            
                            InputPullDown - 13 | 14 - GND                      
                            InputPullDown - 15 | 16 - InputPullDown            
                                vout_3_3V - 17 | 18 - InputPullDown            
                            InputPullDown - 19 | 20 - GND                      
                            InputPullDown - 21 | 22 - InputPullDown            
                            InputPullDown - 23 | 24 - InputPullUp              
                                      GND - 25 | 26 - InputPullUp              
                                    Input - 27 | 28 - InputPullUp              
                              InputPullUp - 29 | 30 - GND                      
                              InputPullUp - 31 | 32 - InputPullDown            
                            InputPullDown - 33 | 34 - GND                      
                            InputPullDown - 35 | 36 - InputPullDown            
                            InputPullDown - 37 | 38 - InputPullDown            
                                      GND - 39 | 40 - InputPullDown            
                """;
            Assert.Equal(expected, pinouts);
        }

        [Fact]
        public void GetPinouts_ShouldReturnUart_WhenUartProtocolEnabled()
        {
            RaspberryPiController sut = new();
            sut.SetProtocol(RaspberryPiController.ProtocolOut.UART, true);
            var pinouts = sut.GetPinouts();
            var expected = """
                                vout_3_3V - 01 | 02 - vout_5v                  
                              InputPullUp - 03 | 04 - vout_5v                  
                              InputPullUp - 05 | 06 - GND                      
                              InputPullUp - 07 | 08 - TxD-UART                 
                                      GND - 09 | 10 - RxD-UART                 
                            InputPullDown - 11 | 12 - InputPullDown            
                            InputPullDown - 13 | 14 - GND                      
                            InputPullDown - 15 | 16 - InputPullDown            
                                vout_3_3V - 17 | 18 - InputPullDown            
                            InputPullDown - 19 | 20 - GND                      
                            InputPullDown - 21 | 22 - InputPullDown            
                            InputPullDown - 23 | 24 - InputPullUp              
                                      GND - 25 | 26 - InputPullUp              
                              InputPullUp - 27 | 28 - InputPullUp              
                              InputPullUp - 29 | 30 - GND                      
                              InputPullUp - 31 | 32 - InputPullDown            
                            InputPullDown - 33 | 34 - GND                      
                            InputPullDown - 35 | 36 - InputPullDown            
                            InputPullDown - 37 | 38 - InputPullDown            
                                      GND - 39 | 40 - InputPullDown            
                """;
            Assert.Equal(expected, pinouts);
        }

        [Fact]
        public void GetPinouts_ShouldReturnUart_WhenSPI1ProtocolEnabled()
        {
            RaspberryPiController sut = new();
            sut.SetProtocol(RaspberryPiController.ProtocolOut.SPI1, true);
            var pinouts = sut.GetPinouts();
            var expected = """
                                vout_3_3V - 01 | 02 - vout_5v                  
                              InputPullUp - 03 | 04 - vout_5v                  
                              InputPullUp - 05 | 06 - GND                      
                              InputPullUp - 07 | 08 - InputPullDown            
                                      GND - 09 | 10 - InputPullDown            
                            InputPullDown - 11 | 12 - InputPullDown            
                            InputPullDown - 13 | 14 - GND                      
                            InputPullDown - 15 | 16 - InputPullDown            
                                vout_3_3V - 17 | 18 - InputPullDown            
                            InputPullDown - 19 | 20 - GND                      
                            InputPullDown - 21 | 22 - InputPullDown            
                            InputPullDown - 23 | 24 - InputPullUp              
                                      GND - 25 | 26 - InputPullUp              
                              InputPullUp - 27 | 28 - InputPullUp              
                              InputPullUp - 29 | 30 - GND                      
                              InputPullUp - 31 | 32 - InputPullDown            
                            InputPullDown - 33 | 34 - GND                      
                                SPI1-MOSI - 35 | 36 - InputPullDown            
                            InputPullDown - 37 | 38 - MISO-SPI1                
                                      GND - 39 | 40 - SCLK-SPI1                
                """;
            Assert.Equal(expected, pinouts);
        }
    }
}