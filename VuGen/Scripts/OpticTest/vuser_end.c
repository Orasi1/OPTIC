vuser_end()
{
	long counterValue;
	
    IncrementCounter("LoadRunner(VUsers)\\Count", -1);	
    
	counterValue = GetCounter("LoadRunner(VUsers)\\Count");

	lr_log_message("LoadRunner(VUsers)\\Count: %d", counterValue);
    
	return 0;
}
