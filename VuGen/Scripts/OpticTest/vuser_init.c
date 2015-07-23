vuser_init()
{
	int retValue;
	long counterValue;

	retValue = lr_load_dll("C:\\Sources\\CodePlex\\Optic\\\Release\\Optic.dll");
	//retValue = lr_load_dll("Optic.dll");
   	lr_log_message("Return value from lr_load_dll(): %d", retValue);
	
//    string baseUrl = "https://orasinfr.saas.appdynamics.com";
//    string credentials = "jon.fowler@orasinfr:Katie123";
//    string applicationName = "Skytap nopCommerce";

	CreateCustomEvent(
		"https://orasinfr.saas.appdynamics.com",
		"jon.fowler@orasinfr:Katie123",
		"Skytap nopCommerce",
		"INFO",
		"OPTIC",
		"Load Test - Start");
   	
	//This only requires read access to registry
   	counterValue = GetCounter("Processor Information(_Total)\\% Processor Time");
   	lr_log_message("Processor Information(_Total)\\% Processor Time: %d", counterValue);

   	//Must have registry write privileges to create a counter
	IncrementCounter("LoadRunner(VUsers)\\Count", 1);

    //Counter must exist or an exception will be raised
	counterValue = GetCounter("LoadRunner(VUsers)\\Count");
   	lr_log_message("LoadRunner(VUsers)\\Count: %d", counterValue);


	return 0;
}

