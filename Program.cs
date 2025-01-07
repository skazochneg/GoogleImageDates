using System.Text.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Diagnostics;

// Get list of files in current directory and go through it
var files = Directory.GetFiles(".");
foreach (var file in files) {
    
    // If this is a jpeg image file
    if ( file.EndsWith("jpeg") || file.EndsWith("jpg") || file.EndsWith("JPG") || file.EndsWith("JPEG"))    
    {
        Console.WriteLine("Jpeg image: " + file);
        
        // and if json file exists
        var jsonFileName = file + ".json";
        if (File.Exists(jsonFileName)) 
        {
            // Console.WriteLine(" ... and JSON exists: " + jsonFileName);

            // Try to read the taken date and time from JSON file
            var jsonText = File.ReadAllText(jsonFileName);
            ImageJsonData? imageJsonData = JsonSerializer.Deserialize<ImageJsonData>(jsonText);
            // Console.WriteLine(" ... FL: " + jsonText.Length + "\tTimestamp: " + imageJsonData?.photoTakenTime?.timestamp + "\tFormatted: " + imageJsonData?.photoTakenTime?.formatted);
            
            // Разберёмся с датами
            var unixTimeStamp = imageJsonData?.photoTakenTime?.timestamp;
            if (unixTimeStamp is null) continue;
            DateTime correctDateTime;
            try
            {
                correctDateTime = UnixTimeStampToDateTime(Convert.ToDouble(unixTimeStamp)); // DateTime.ParseExact(formatedDate,"r",culture);
                Console.WriteLine(" ... Managed to get Date and Time: " + correctDateTime.ToString() + " from Unix Time Stamp " + unixTimeStamp);
            }
            catch
            {
                Console.WriteLine(" ... Could not parse Date and Time from: " + unixTimeStamp);
                continue;
            }
            
            // Теперь работаем с картинкой
            var image = new Bitmap(file);
            
            PropertyItem[] propItems = image.PropertyItems;
            
            Encoding _Encoding = Encoding.UTF8;
            try 
            {
                PropertyItem dataTakenProperty = propItems[0];
                dataTakenProperty.Id = 0x9003;  // PropertyTagExifDTOrig	0x9003
                dataTakenProperty.Value = _Encoding.GetBytes(correctDateTime.ToString("yyyy:MM:dd HH:mm:ss") + '\0');
                dataTakenProperty.Type = 2; // a null-terminated ASCII string, see https://learn.microsoft.com/en-us/dotnet/api/system.drawing.imaging.propertyitem.type?view=windowsdesktop-9.0
                dataTakenProperty.Len = dataTakenProperty.Value.Length; // Need to set manually
                image.SetPropertyItem(dataTakenProperty);
                dataTakenProperty.Id = 0x9004;  // PropertyTagExifDTDigitized    0x9004
                image.SetPropertyItem(dataTakenProperty);
                image.Save(file + "_u.jpeg");
                image.Dispose();
            }
            catch
            {
                Debug.WriteLine("No date property found");
            }

            
      
        }
    }
}

static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
{
    // Unix timestamp is seconds past epoch
    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
    return dateTime;
}

/*
private DateTime ReadDate(String fileName) {

    Image theImage = new Bitmap(fileName);
    PropertyItem[] propItems = theImage.PropertyItems;
    Encoding _Encoding = Encoding.UTF8;
    var DataTakenProperty1 = propItems.Where(a => a.Id.ToString("x") == "9004").FirstOrDefault();
    var DataTakenProperty2 = propItems.Where(a => a.Id.ToString("x") == "9003").FirstOrDefault();
    string originalDateString = _Encoding.GetString(DataTakenProperty1.Value);
    originalDateString = originalDateString.Remove(originalDateString.Length - 1);
    DateTime originalDate = DateTime.ParseExact(originalDateString, "yyyy:MM:dd HH:mm:ss", null);

    return originalDate;

//    originalDate = originalDate.AddHours(-7);

    DataTakenProperty1.Value = _Encoding.GetBytes(originalDate.ToString("yyyy:MM:dd HH:mm:ss") + '\0');
    DataTakenProperty2.Value = _Encoding.GetBytes(originalDate.ToString("yyyy:MM:dd HH:mm:ss") + '\0');
    theImage.SetPropertyItem(DataTakenProperty1);
    theImage.SetPropertyItem(DataTakenProperty2);
    string new_path = System.IO.Path.GetDirectoryName(path) + "\\_" + System.IO.Path.GetFileName(path);
    theImage.Save(new_path);
    theImage.Dispose();
  
}
*/

class ImageJsonData
{
    public photoTakenTime? photoTakenTime { get; set; }
}

class photoTakenTime
{
    public string? timestamp { get; set; }
    public string? formatted { get; set; }
}