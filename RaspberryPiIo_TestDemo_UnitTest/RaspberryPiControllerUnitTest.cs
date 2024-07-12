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
    }
}