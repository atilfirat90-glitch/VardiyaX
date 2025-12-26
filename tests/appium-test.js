const { remote } = require('webdriverio');

const capabilities = {
    platformName: 'Android',
    'appium:automationName': 'UiAutomator2',
    'appium:deviceName': 'emulator-5554',
    'appium:appPackage': 'com.vardiyax.mobile',
    'appium:appActivity': 'crc640e0fe095e31d649d.MainActivity',
    'appium:noReset': true,
    'appium:newCommandTimeout': 300
};

const wdOpts = {
    hostname: 'localhost',
    port: 4723,
    logLevel: 'info',
    capabilities
};

async function runTest() {
    console.log('Starting Appium test...');
    const driver = await remote(wdOpts);
    
    try {
        console.log('Waiting for app to load...');
        await driver.pause(5000);
        
        // AutomationId kullanarak elementleri bul (MAUI'de resource-id olarak görünür)
        // MAUI AutomationId formatı: com.vardiyax.mobile:id/AutomationId
        
        // Username alanını AutomationId ile bul
        console.log('Finding username field by AutomationId...');
        let usernameField = await driver.$('~UsernameEntry'); // accessibility id
        if (!await usernameField.isExisting()) {
            // resource-id ile dene
            usernameField = await driver.$('//*[@resource-id="UsernameEntry"]');
        }
        if (!await usernameField.isExisting()) {
            // content-desc ile dene
            usernameField = await driver.$('//*[@content-desc="UsernameEntry"]');
        }
        if (!await usernameField.isExisting()) {
            // Fallback: placeholder text ile
            usernameField = await driver.$('//android.widget.EditText[contains(@text, "username") or contains(@hint, "username")]');
        }
        if (!await usernameField.isExisting()) {
            // Son çare: ilk EditText
            usernameField = await driver.$('//android.widget.EditText[1]');
        }
        
        await usernameField.waitForDisplayed({ timeout: 10000 });
        await usernameField.click();
        await usernameField.clearValue();
        await usernameField.setValue('admin');
        console.log('Username entered: admin');
        
        // Password alanını AutomationId ile bul
        console.log('Finding password field by AutomationId...');
        let passwordField = await driver.$('~PasswordEntry');
        if (!await passwordField.isExisting()) {
            passwordField = await driver.$('//*[@resource-id="PasswordEntry"]');
        }
        if (!await passwordField.isExisting()) {
            passwordField = await driver.$('//*[@content-desc="PasswordEntry"]');
        }
        if (!await passwordField.isExisting()) {
            // Fallback: password attribute ile
            passwordField = await driver.$('//android.widget.EditText[@password="true"]');
        }
        if (!await passwordField.isExisting()) {
            // Son çare: ikinci EditText
            passwordField = await driver.$('//android.widget.EditText[2]');
        }
        
        await passwordField.waitForDisplayed({ timeout: 5000 });
        await passwordField.click();
        await passwordField.clearValue();
        await passwordField.setValue('dKhEH5xdy5r7');
        console.log('Password entered');
        
        // Klavyeyi kapat
        try {
            await driver.hideKeyboard();
        } catch (e) {
            console.log('Keyboard already hidden or not available');
        }
        
        // Login butonunu AutomationId ile bul
        console.log('Finding login button by AutomationId...');
        let loginButton = await driver.$('~LoginButton');
        if (!await loginButton.isExisting()) {
            loginButton = await driver.$('//*[@resource-id="LoginButton"]');
        }
        if (!await loginButton.isExisting()) {
            loginButton = await driver.$('//*[@content-desc="LoginButton"]');
        }
        if (!await loginButton.isExisting()) {
            // Fallback: text ile
            loginButton = await driver.$('//android.widget.Button[@text="Login"]');
        }
        if (!await loginButton.isExisting()) {
            loginButton = await driver.$('//android.widget.Button[contains(@text, "Login")]');
        }
        
        await loginButton.waitForDisplayed({ timeout: 5000 });
        await loginButton.click();
        console.log('Login button clicked');
        
        // Giriş sonrası bekle
        await driver.pause(5000);
        
        // Ana sayfa kontrolü
        console.log('Checking if login successful...');
        const pageSource = await driver.getPageSource();
        
        if (pageSource.includes('Çalışanlar') || pageSource.includes('Employees') || 
            pageSource.includes('Vardiyalar') || pageSource.includes('Schedules')) {
            console.log('✅ LOGIN SUCCESSFUL! Main page loaded.');
            
            // Hamburger menüyü test et
            console.log('\n--- Testing Hamburger Menu ---');
            
            // FlyoutIcon'a tıkla (hamburger menu)
            let hamburgerMenu = await driver.$('~FlyoutIcon');
            if (!await hamburgerMenu.isExisting()) {
                hamburgerMenu = await driver.$('//android.widget.ImageButton[@content-desc="Open navigation drawer"]');
            }
            if (!await hamburgerMenu.isExisting()) {
                hamburgerMenu = await driver.$('//android.widget.ImageButton[1]');
            }
            
            if (await hamburgerMenu.isExisting()) {
                await hamburgerMenu.click();
                console.log('Hamburger menu opened');
                await driver.pause(2000);
                
                // Kullanıcı Yönetimi'ne tıkla
                const usersMenu = await driver.$('//*[contains(@text, "Kullanıcı") or contains(@text, "User")]');
                if (await usersMenu.isExisting()) {
                    await usersMenu.click();
                    console.log('Users page opened');
                    await driver.pause(3000);
                    
                    const usersPageSource = await driver.getPageSource();
                    if (!usersPageSource.includes('Exception') && !usersPageSource.includes('Error')) {
                        console.log('✅ Users page loaded without crash!');
                    } else {
                        console.log('❌ Users page has errors');
                    }
                }
            }
            
        } else if (pageSource.includes('Login') || pageSource.includes('Username')) {
            console.log('❌ LOGIN FAILED - Still on login page');
            
            // Hata mesajı kontrolü - AutomationId ile
            let errorLabel = await driver.$('~ErrorLabel');
            if (!await errorLabel.isExisting()) {
                errorLabel = await driver.$('//*[@resource-id="ErrorLabel"]');
            }
            if (await errorLabel.isExisting()) {
                const errorText = await errorLabel.getText();
                console.log('Error message:', errorText);
            }
            
            if (pageSource.includes('Hata') || pageSource.includes('Error') || pageSource.includes('Invalid')) {
                console.log('Error message found on page');
            }
        } else {
            console.log('⚠️ Unknown state - checking page source...');
            console.log(pageSource.substring(0, 1000));
        }
        
    } catch (error) {
        console.error('Test error:', error.message);
        
        // Hata durumunda UI dump al
        try {
            const pageSource = await driver.getPageSource();
            console.log('\n--- Page Source on Error ---');
            console.log(pageSource.substring(0, 2000));
        } catch (e) {
            console.log('Could not get page source');
        }
    } finally {
        await driver.deleteSession();
        console.log('\nTest completed.');
    }
}

runTest();
