window.setCookie = (name, value, days, domain) => {
    let expires = "";
    if (days) {
        const date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    var sameSiteString = "; SameSite=None";
    var secureString = "; Secure=True";

    var domainString = domain ? "; Domain=" + domain : "";

    document.cookie = name + "=" + (value || "") + expires + "; path=/" + sameSiteString + secureString + domainString;

}

window.deleteCookie = (name) => {
    document.cookie = name + '=; Max-Age=-99999999;';
}

window.getCookie = (name) => {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
}
