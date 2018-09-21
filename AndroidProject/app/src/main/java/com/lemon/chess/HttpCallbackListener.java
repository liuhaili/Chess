package com.lemon.chess;

public interface HttpCallbackListener {

    /**
     * 服务器响应成功时调用，根据返回的内容在里面处理逻辑
     * @param response
     */
    void onFinish(String response);
    /**
     * 网络操作出现错误的时候调用，在里面对异常情况进行处理
     * @param e
     */
    void onError(Exception e);


}
