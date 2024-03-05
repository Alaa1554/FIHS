import api, { userApi } from '@/utils/api';
import { useMutation } from '@tanstack/react-query';
import useSession, { DEFAULT_SESSION } from './state/useSession';
import { useRouter } from 'expo-router';
import Toast from 'react-native-toast-message';
import { AxiosResponse } from 'axios';
import storage from '@/utils/storage';

const useLogin = ()=>{    
    const { setSession, setLoading } = useSession()
    const router = useRouter()
    return useMutation({
        mutationFn: async({email, password}:{email: string, password: string}): Promise<any>=> {
            setLoading(true)    
            await api.post<Session, AxiosResponse<Session, {msg: string}>>(`/Auth/login`, {email:email, password:password},{
                headers:{
                    'Content-Type':'application/json'
                }
            }).then((res)=>{
                let rt = res.headers['set-cookie']?.[0]
                storage.save({
                    key:'refreshToken',
                    data:rt
                })
                setSession(res.data)
                setLoading(false)
                router.replace('/(tabs)/home')
            }).catch((err)=>{
                setLoading(false)
                Toast.show({
                    type:'error',
                    text1:'خطأ',
                    text2:err.response.data
                })
                console.log(err.response);
            })
        }
    })
}
export default useLogin

export const Logout = ()=>{    
    const { setSession, setLoading, token } = useSession()
    const rf = storage.load({key:'refreshToken'})
    const router = useRouter()
    return useMutation({
        mutationFn: async(): Promise<any>=> {
            setLoading(true)    
            await userApi(token, await rf).get<Session, AxiosResponse<Session, {msg: string}>>(`/Auth/logout`,).then((res)=>{
                storage.remove({key:'refreshToken'})                
                //@ts-ignore
                setSession(DEFAULT_SESSION)
                setLoading(false)
                // router.replace('/(auth)/login')
            }).catch((err)=>{
                setLoading(false)
                Toast.show({
                    type:'error',
                    text1:'خطأ',
                    text2:err.response.data
                })
                console.log(err.response);
            })
        }
    })
}