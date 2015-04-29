Action()
{
	double trans_time;
	long counterValue;

	/* <comments> */
	//Count, Rate/Sec, Average
	lr_start_transaction("MyTransactionName");
	trans_time=lr_get_transaction_duration("MyTransactionName");
	
	lr_log_message("trans_time: %f", trans_time);
	//lr_rendezvous("");

	IncrementCounter("PRLoadRunner2(MyTransactionName)\\Rate/Sec", 1);	
	IncrementCounter("PRLoadRunner2(MyTransactionName)\\Average", trans_time);
	lr_end_transaction("MyTransactionName", LR_AUTO);

	counterValue = GetCounter("Processor Information(_Total)\\% Processor Time");
   	lr_log_message("Processor Information(_Total)\\% Processor Time: %d", counterValue);
	return 0;
}
