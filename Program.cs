using Dynastream.Fit;

internal class Program
{
    public static void Main(string[] args)
    {
        var Filename = "test.FIT";
        ushort ProductId = 1;
        // 1. Create the output stream, this can be any type of stream, including a file or memory stream. Must have read/write access.
        FileStream fitDest = new FileStream(Filename, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

        // 2. Create a FIT Encode object.
        Encode encoder = new Encode(ProtocolVersion.V10);

        // 3. Write the FIT header to the output stream.
        encoder.Open(fitDest);

        // The timestamp for the workout file
        var timeCreated = new Dynastream.Fit.DateTime(System.DateTime.UtcNow);

        // 4. Every FIT file MUST contain a File ID message as the first message
        var fileIdMesg = new FileIdMesg();
        fileIdMesg.SetType(Dynastream.Fit.File.Workout);
        fileIdMesg.SetManufacturer(Manufacturer.Development);
        fileIdMesg.SetProduct(ProductId);
        fileIdMesg.SetSerialNumber(timeCreated.GetTimeStamp());
        fileIdMesg.SetTimeCreated(timeCreated);
        encoder.Write(fileIdMesg);

        // 5. Every FIT Workout file MUST contain a Workout message as the second message
        var workoutMesg = new WorkoutMesg();
        workoutMesg.SetWktName("Bike Workout");
        workoutMesg.SetSport(Sport.Cycling);
        workoutMesg.SetSubSport(SubSport.Invalid);
        workoutMesg.SetNumValidSteps(1);
        encoder.Write(workoutMesg);

        // 6. Every FIT Workout file MUST contain one or more Workout Step messages
        var workoutStepMesg = new WorkoutStepMesg();
        workoutStepMesg.SetMessageIndex(0);
        workoutStepMesg.SetWktStepName("Endurance Ride");
        workoutStepMesg.SetNotes("Keep your HR in Zone 2 for the entire ride.");
        workoutStepMesg.SetIntensity(Intensity.Active);
        workoutStepMesg.SetDurationType(WktStepDuration.Time);
        workoutStepMesg.SetDurationValue(36000000);
        workoutStepMesg.SetTargetType(WktStepTarget.HeartRate);
        workoutStepMesg.SetTargetValue(2);
        encoder.Write(workoutStepMesg);

        // 7. Update the data size in the header and calculate the CRC
        encoder.Close();

        // 8. Close the output stream
        fitDest.Close();

        Console.WriteLine($"Encoded FIT file '{Filename}'");
    }
}
