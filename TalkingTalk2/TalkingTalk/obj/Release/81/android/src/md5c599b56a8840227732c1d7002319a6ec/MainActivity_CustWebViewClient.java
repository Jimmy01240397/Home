package md5c599b56a8840227732c1d7002319a6ec;


public class MainActivity_CustWebViewClient
	extends android.webkit.WebChromeClient
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onProgressChanged:(Landroid/webkit/WebView;I)V:GetOnProgressChanged_Landroid_webkit_WebView_IHandler\n" +
			"";
		mono.android.Runtime.register ("TalkingTalk.MainActivity+CustWebViewClient, TalkingTalk", MainActivity_CustWebViewClient.class, __md_methods);
	}


	public MainActivity_CustWebViewClient ()
	{
		super ();
		if (getClass () == MainActivity_CustWebViewClient.class)
			mono.android.TypeManager.Activate ("TalkingTalk.MainActivity+CustWebViewClient, TalkingTalk", "", this, new java.lang.Object[] {  });
	}

	public MainActivity_CustWebViewClient (md5c599b56a8840227732c1d7002319a6ec.MainActivity p0)
	{
		super ();
		if (getClass () == MainActivity_CustWebViewClient.class)
			mono.android.TypeManager.Activate ("TalkingTalk.MainActivity+CustWebViewClient, TalkingTalk", "TalkingTalk.MainActivity, TalkingTalk", this, new java.lang.Object[] { p0 });
	}


	public void onProgressChanged (android.webkit.WebView p0, int p1)
	{
		n_onProgressChanged (p0, p1);
	}

	private native void n_onProgressChanged (android.webkit.WebView p0, int p1);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
