using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

/*
 * LOLLO NOTE
 * XML serialization does not convert methods, indexers, private fields, or read-only properties (except read-only collections). 
 * To serialize all an object's fields and properties, both public and private, 
 * use the DataContractSerializer instead of XML serialization. 
 * https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.datacontractserializer?view=netframework-4.7
 */

/*
 * A serialisation produces
    <Data0101>
		<Var01>var01</Var01>
		<_var01>var01</_var01>
		<_var02>var02</_var02>
	</Data0101>
    OR <Data0101 i:nil="true"/> if I never initialise Data0101
	<LastMessage/>
	<Var01>var01</Var01>
	<Var02>var02</Var02>
	<Var03>var03</Var03>
	<_lastMessage/>
	<_var01>var01</_var01>
	<_var04>var04</_var04>
	<_var05>var05</_var05>
	<_var06>var06</_var06>
	<_var07>var07</_var07>

    A subsequent deserialisation fills all the fields and properties that were serialised, the others are null.
    If Data0101 was never initialised, it is also null, logically.

    There seems to be no need for a public parameterless constructor.
    DataContractSerializer looks immune from readonly; 
    it only suffers if a setter is missing and a property is decorated with [DataMember], 
    but then you can serialize (and decorate) its _backing field instead.

    But then, why do I need [IgnoreDataMember]?
 */
namespace SerialisationDeserialisation
{
    [DataContract]
    public class Data01
    {
        [DataMember]
        private string _lastMessage = string.Empty;
        [DataMember]
        public string LastMessage { get { return _lastMessage; } set { _lastMessage = value; } }

        [DataMember]
        private string _var01 = "var01";
        [DataMember]
        public string Var01
        {
            get { return _var01; }
            set { _var01 = value; }
        }

        private string _var02 = "var02";
        [DataMember]
        public string Var02
        {
            get { return _var02; }
            private set { _var02 = value; }
        }

        private string _var03 = "var03";
        [DataMember]
        private string Var03
        {
            get { return _var03; }
            set { _var03 = value; }
        }
        [DataMember]
        private string _var04 = "var04";
        // cannot use [DataMember] coz there is no setter; DataContractSerializer just ignores it
        public string Var04
        {
            get { return _var04; }
        }
        [DataMember]
        private string _var05 = "var05";
        // cannot use [DataMember] coz there is no setter; DataContractSerializer just ignores it
        private string Var05
        {
            get { return _var05; }
        }
        [DataMember]
        private readonly string _var06 = "var06";
        private string Var06
        {
            get { return _var06; }
        }
        [DataMember]
        private volatile string _var07 = "var07";
        private string Var07
        {
            get { return _var07; }
        }

        private string _var08 = "var08";
        //[DataMember] // if [DataMember] is missing, DataContractSerializer ignores it, no need for [IgnoreDataMember]
        private string Var08
        {
            get { return _var08; }
            set { _var08 = value; }
        }
        public string _var09 = "var09";
        //[DataMember] // if [DataMember] is missing, DataContractSerializer ignores it, no need for [IgnoreDataMember]
        public string Var09
        {
            get { return _var09; }
            set { _var09 = value; }
        }

        private string _var10 = "var10";
        [IgnoreDataMember]
        private string Var10
        {
            get { return _var10; }
            set { _var10 = value; }
        }
        public string _var11 = "var11";
        [IgnoreDataMember]
        public string Var11
        {
            get { return _var11; }
            set { _var11 = value; }
        }

        private Data0101 _data0101;
        [DataMember]
        public Data0101 Data0101
        {
            get { return _data0101; }
            set { _data0101 = value; }
        }

        public void SetVar01(string newValue)
        {
            Var01 = newValue;
        }
        public void SetVar02(string newValue)
        {
            Var02 = newValue;
        }
        public void SetVar03(string newValue)
        {
            Var03 = newValue;
        }
        public void SetVar04(string newValue)
        {
            _var04 = newValue;
        }
        public void SetVar05(string newValue)
        {
            _var05 = newValue;
        }

        private static Data01 _instance;
        private static readonly object _instanceLocker = new object();
        public static Data01 GetInstance()
        {
            lock (_instanceLocker)
            {
                return _instance ?? (_instance = new Data01());
            }
        }
        private Data01()
        {
            //Data0101 = new Data0101(); 
        }
    }
    [DataContract]
    public class Data0101
    {
        [DataMember]
        private string _var01 = "var01";
        [DataMember]
        public string Var01
        {
            get { return _var01; }
            set { _var01 = value; }
        }
        [DataMember]
        private readonly string _var02 = "var02";

        public string Var02
        {
            get { return _var02; }
            //set { _var02 = value; }
        }

    }
}
