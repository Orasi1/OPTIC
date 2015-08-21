Action()
{
	lr_start_transaction("LDB_010_Connect");

	//OPTIC: Adds a LoadRunner header that AppDynamics will recognize.
	web_add_header("AppDHeader", "LDB_010_Connect");

	lr_end_transaction("LDB_010_Connect", LR_AUTO);
	
	return 0;
}
