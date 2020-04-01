# Emby.Facebook
Post Emby server data to a facebook page


![alt text](https://github.com/chefbennyj1/Emby.Facebook/blob/master/Facebook/thumb.png?raw=true)




This plugin will post likes and watch information to a Facebook page.

This plugin will post likes and watch information to a Facebook page.

 

This plugin will not post data to a person Facebook page, it will post to a page you create (either a business page or personal)

Note: Please let me know if I have completely bumbled the instructions below.

 

Step One:

 

Create the page from your Facebook Account:

 
![alt text](https://emby.media/community/uploads/inline/27/5e8243c8b5b1b_stepone.png)


 

![alt text](https://emby.media/community/uploads/inline/27/5e8243dad8ac1_steptwo.png)

 

 

 

Step two:

 

Once the page is set up you can open the plugin settings, and select the link to go to Facebook API setup.

 

![alt text](https://emby.media/community/uploads/inline/27/5e824408829cc_stepthree.png)

 

Step three:

 

![alt text](https://emby.media/community/uploads/inline/27/5e8244af717bc_stepfour.png)

The page above will ask you to login and link your account.

 

 

1. Select the follow permissions:

 

manage_pages

publish_pages

 

2. Under "User or Page" select the page that you have created

 

3. Press the "Generate Access Token" button.

 

 

This Access token (API Key) will only last a couple hours! This is not good enough!

 

We want a token that will never expire!

 

To do this we want follow these steps:

 

1.  Select the blue button beside your access token:

 

![alt text](https://emby.media/community/uploads/inline/27/5e8245c6953a7_step5.png)

 

2. Select "Open In Access Token Tool":

 

![alt text](https://emby.media/community/uploads/inline/27/5e824633bb8dd_step6.png)

 

3. Select "Extended Access Token"

 

![alt text](https://emby.media/community/uploads/inline/27/5e82467fec678_step7.png)

 

This will create an access token that will last 6 months, but that is still not good enough! We want an never ending Token.

 

3. Select the "Debug" button at the bottom of your screen:

 

![alt text](https://emby.media/community/uploads/inline/27/5e82472478e43_step8.png)

 

4. There is our access token that never expires!

 

![alt text](https://emby.media/community/uploads/inline/27/5e824789a881f_step9.png)

 

5. Copy and paste that into the Plugin Configuration Page and Press the Save button.

 

 

 

This is a beta plugin.

 

 

What the Page will do!

 

 

![alt text](https://emby.media/community/uploads/inline/27/5e82483cb69ac_step10.png)

 

 

 

![alt text](https://emby.media/community/uploads/inline/27/5e82486dd8398_step11.png)

 

 

Note: The Facebook API Access Token can get a little messy. Just make sure that you have choosen your 'page' as the recipient of the Access Token.

 

The Facebook API will not allow any Posts from the API rto personal pages without a Registered App, which I have already requested permissions top get, but because of COVID 19 it might take a while to get it registered.

 

If this is interesting to anyone, I'll continue development and try to make things easier. 
