vuser_init()
{
	//Initialize Optic
	lr_load_dll("Optic.dll");

	//Increment VUsers
	IncrementCounter("LoadRunner(VUsers)\\Count", 1);
	
	CreateCustomEvent(
	"https://<controller URL",
	"your_username>@customer1:your_password",
	"Application Name",
	"INFO",
	"OPTIC",
	"Load Test - Start");
	
	return 0;
}

