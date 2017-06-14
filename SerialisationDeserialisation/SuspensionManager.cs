using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;

namespace SerialisationDeserialisation
{
    public static class SuspensionManager
    {
        private const string SettingsFilename = "LolloSessionData.xml";
        //private static readonly Type[] KnownTypes = {typeof(IReadOnlyList<string>), typeof(string[])};

        public static async Task<Data01> LoadSettingsAsyncV1() //List<string> errorMessages)
        {
            string errorMessage = string.Empty;
            Data01 newData01 = null;

            try
            {
                StorageFile file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(SettingsFilename, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);

                //string ssss = null; //this is useful when you debug and want to see the file as a string
                //using (IInputStream inStream = await file.OpenSequentialReadAsync())
                //{
                //    using (StreamReader streamReader = new StreamReader(inStream.AsStreamForRead()))
                //    {
                //      ssss = streamReader.ReadToEnd();
                //    }
                //}

                using (var inStream = await file.OpenSequentialReadAsync().AsTask().ConfigureAwait(false))
                {
                    using (var iinStream = inStream.AsStreamForRead())
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(Data01)); //, KnownTypes);
                        iinStream.Position = 0;
                        newData01 = (Data01)(serializer.ReadObject(iinStream));
                        await iinStream.FlushAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (XmlException exc)
            {
                errorMessage = $"XmlException: could not restore the settings: {exc?.Message}";
                newData01 = Data01.GetInstance();
            }
            catch (Exception exc)
            {
                errorMessage = $"could not restore the settings: {exc?.Message}";
                newData01 = Data01.GetInstance();
            }
            finally
            {
                newData01.LastMessage = errorMessage;
            }
            return newData01;
        }
        // this one fails
        public static async Task<Data01> LoadSettingsAsyncV2() //List<string> errorMessages)
        {
            string errorMessage = string.Empty;
            Data01 newData01 = null;

            try
            {
                StorageFile file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(SettingsFilename, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);

                //string ssss = null; //this is useful when you debug and want to see the file as a string
                //using (IInputStream inStream = await file.OpenSequentialReadAsync())
                //{
                //    using (StreamReader streamReader = new StreamReader(inStream.AsStreamForRead()))
                //    {
                //      ssss = streamReader.ReadToEnd();
                //    }
                //}

                using (var inStream = await file.OpenSequentialReadAsync().AsTask().ConfigureAwait(false))
                {
                    using (var iinStream = inStream.AsStreamForRead())
                    {
                        using (var reader = XmlDictionaryReader.CreateTextReader(iinStream, new XmlDictionaryReaderQuotas()))
                        {

                            DataContractSerializer serializer = new DataContractSerializer(typeof(Data01)); //, KnownTypes);
                            iinStream.Position = 0;

                            newData01 = (Data01)serializer.ReadObject(reader, true);
                        }
                        await iinStream.FlushAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (XmlException exc)
            {
                errorMessage = $"XmlException: could not restore the settings: {exc?.Message}";
                newData01 = Data01.GetInstance();
            }
            catch (Exception exc)
            {
                errorMessage = $"could not restore the settings: {exc?.Message}";
                newData01 = Data01.GetInstance();
            }
            finally
            {
                newData01.LastMessage = errorMessage;
            }
            return newData01;
        }

        public static async Task SaveSettingsAsync(Data01 data01)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var sessionDataSerializer = new DataContractSerializer(typeof(Data01));
                    // DataContractSerializer serializer = new DataContractSerializer(typeof(Data01), new DataContractSerializerSettings() { SerializeReadOnlyTypes = true });
                    // DataContractSerializer sessionDataSerializer = new DataContractSerializer(typeof(Data01), _knownTypes);
                    // DataContractSerializer sessionDataSerializer = new DataContractSerializer(typeof(Data01), new DataContractSerializerSettings() { KnownTypes = _knownTypes, SerializeReadOnlyTypes = true, PreserveObjectReferences = true });
                    sessionDataSerializer.WriteObject(memoryStream, data01);

                    var sessionDataFile = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(
                        SettingsFilename, CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false);
                    using (Stream fileStream = await sessionDataFile.OpenStreamForWriteAsync().ConfigureAwait(false))
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        await memoryStream.CopyToAsync(fileStream).ConfigureAwait(false);
                        await memoryStream.FlushAsync().ConfigureAwait(false);
                        await fileStream.FlushAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
        }
    }

}
