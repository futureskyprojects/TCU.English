# TCU ENGLISH PROJECT
### Set up VPS
-- https://jobs.hybrid-technologies.vn/blog/huong-dan-su-dung-docker-co-ban/
# A. Create your own VPS
## 1. Access "vultr.com" and create your account.
## 2. In "https://my.vultr.com/" > Products (Or access "https://my.vultr.com/deploy/") create VPS

# B. Access VPS using SSH
## 1. Open Terminal > type command "ssh root@<VPS_IP>"

# C. Install Docker
## 1. Enable Docker CE Repository
``` dnf config-manager --add-repo=https://download.docker.com/linux/centos/docker-ce.repo ```
## 2. Install Docker CE using dnf command
``` dnf install docker-ce --nobest -y ```
## 3. Start Docker (Automatically Start)
``` sudo systemctl start docker ```
``` systemctl enable docker ```
## 4. Verify and test Docker CE Engine
``` docker run hello-world ```

# D. Install MySQL
## 1. Pull the MySQL Docker Image
``` docker pull mysql/mysql-server:latest ```
## 2. Verify MySQL Images
``` docker images ```
## 3. Deploy the MySQL Container
``` docker run --name=[container_name] -d mysql/mysql-server:latest ```