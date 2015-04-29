vuser_init()
{
	int retval = 0;
	long counterValue;

	retval = lr_load_dll("C:\\sources\\github\\OrasiAppDynamicsLoadToolsIntegration\\Release\\Optic.dll");
    IncrementCounter("PRLoadRunner2(VUsers)\\Count", 1);

    counterValue = GetCounter("PRLoadRunner2(VUsers)\\Count");
   	lr_log_message("PRLoadRunner2(VUsers)\\Count: %d", counterValue);

	counterValue = GetCounter("Processor Information(_Total)\\% Processor Time");
   	lr_log_message("Processor Information(_Total)\\% Processor Time: %d", counterValue);

//   	counterValue = GetCounter("Process(services)\\Handle Count");
//   	lr_log_message("counter: %d", counterValue);

	return 0;
}
