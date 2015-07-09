vuser_init()
{
	int retval = 0;
	long counterValue;

	retval = lr_load_dll("C:\\Users\\conjpf\\Sources\\Codeplex\\Optic2\\Release\\Optic.dll");
	
    IncrementCounter("LoadRunner(VUsers)\\Count", 1);

    counterValue = GetCounter("LoadRunner(VUsers)\\Count");
   	lr_log_message("LoadRunner(VUsers)\\Count: %d", counterValue);

	counterValue = GetCounter("Processor Information(_Total)\\% Processor Time");
   	lr_log_message("Processor Information(_Total)\\% Processor Time: %d", counterValue);

//   	counterValue = GetCounter("Process(services)\\Handle Count");
//   	lr_log_message("counter: %d", counterValue);

	return 0;
}
