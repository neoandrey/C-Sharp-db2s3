
using  System;
using  System.Collections;
using  System.Collections.Generic;
using  System.Text;

namespace db2s3
{

    public class S3UploadConfig{
        public string       directory_of_upload_files  { set; get;}
        public  List<S3Gateway>    s3_gateways { set; get;}
        public string       log_file_name { set; get;}
        public string       bucket_name  { set; get;}
        public string       server_name  { set; get;}
        public string       server_ip_address  { set; get;}
        public string       additional_server_info { set; get;}

        public string      sqlite_database_name {set; get;}
        public string      sqlite_database_file { set; get;}          
        public string      access_id { set; get;}
        public string      access_key { set; get;}
        public string      profile_path { set; get;}
        public string      profile_name {set; get;}
        public bool        use_profile_file { set; get;}
        public string      central_inventory_server { set; get;}
        public string      central_inventory_user { set; get;}
        public string      central_inventory_password { set; get;}
        public string      central_inventory_database_name { set; get;}
        public bool        run_on_schedule { set; get;}
        public string      schedule_name { set; get;}
        public bool        send_notification { set; get;}
        public string      to_address  { set; get;}
        public string      from_address { set; get;}
        public string      bcc_address  { set; get;}
        public string      cc_address  { set; get;}
        public string      smtp_server  { set; get;}
        public int         smtp_port   { set; get;}
        public string      sender     { set; get;}
        public string      sender_password  { set; get;}
        public bool        is_ssl_enabled  { set; get;}
        public string      alternate_row_colour{set; get;}
        public string      email_font_family{set; get;}
        public string      email_font_size{set; get;}
        public string      colour {set; get;}
        public string      border_colour{set; get;}
        public string      sqlite_config {set; get;}

        public string      region {set; get;}
        

    }
}