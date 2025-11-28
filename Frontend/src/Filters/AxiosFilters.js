import axios from "axios";

var axiosInstance = axios.create();

axiosInstance.interceptors.request.use(
    (config)=>
    {
        var user =  JSON.parse(sessionStorage.getItem("user"))
        if(user && user.token)
        {
            config.headers["Authorization"] = "Bearer "+user.token
        }
        return config;
    },(error)=>{
        return Promise.reject(error);
    }
)

 export default axiosInstance;